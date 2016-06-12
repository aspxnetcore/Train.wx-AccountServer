
$.init();

//常量数据
//分类page
var myCache = {
	shopOrderby: [
		{ ID: "", Key: "更新" },
		{ ID: "new", Key: "新店" },
		{ ID: "level", Key: "等级" },
		{ ID: "hot", Key: "粉丝量" },
		{ ID: "name", Key: "名称" },
		{ ID: "namedesc", Key: "名称反序" }

	],
	zqWhereData: [
        { ID: "", Key: "全部" },
        { ID: "unreceived", Key: "未领" },
		{ ID: "received", Key: "已领" },
	],

	dynamicLin: [],
	classesData: [],
	T7s: {} //{urlName,T7}
	, frameGetT7: function (page, index) {
		if (index == undefined) { index = 0 };
		if (myCache.T7s[page.name]) {
			return myCache.T7s[page.name][index]
		} else {
			var t7Tags = $(page.container).find("#contentT7");
			if (t7Tags == null || t7Tags.length == 0) {
				throw new Error("找不到t7的模块")
				return;
			} else {
				myCache.T7s[page.name] = [];
				t7Tags.each(function (i) {
					myCache.T7s[page.name][i] = Template7.compile($$(this).html());
				})
			}
			return myCache.T7s[page.name][index]
		}

	}
}
var myFun = {
	myH5Upload: {
		dataURLtoBlob: function (dataurl) {
			var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
                bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
			while (n--) {
				u8arr[n] = bstr.charCodeAt(n);
			}
			return new Blob([u8arr], { type: mime });
		},
		v1: function (it, imgTarget) {
			if (typeof (MegaPixImage) == "undefined")
				myFun.myH5Upload.MegaPixImageInit();

			var file = it.files[0]
			if (!file) {
				return;
			}
			var $it = $$(it);
			var _reader = new FileReader();
			//var _canvas = document.createElement("canvas");
			var mpi = new MegaPixImage(file);
			var _img = new Image();
			mpi.render(_img, { maxWidth: 400, maxHeight: 400, quality: .6 }, function (e) {
				it.parentNode.style.backgroundImage = "url(" + _img.src + ")";
				//$it.parent().css("background-image","url("+ e.srcImage.src+")");
				$it.parent().data("uppic", _img.src);

				//alert(_canvas.toBlob())
			});


		},
		MegaPixImageInit: function () {


			/**
			 * Detect subsampling in loaded image.
			 * In iOS, larger images than 2M pixels may be subsampled in rendering.
			 */
			function detectSubsampling(img) {
				var iw = img.naturalWidth, ih = img.naturalHeight;
				if (iw * ih > 1024 * 1024) { // subsampling may happen over megapixel image
					var canvas = document.createElement('canvas');
					canvas.width = canvas.height = 1;
					var ctx = canvas.getContext('2d');
					ctx.drawImage(img, -iw + 1, 0);
					// subsampled image becomes half smaller in rendering size.
					// check alpha channel value to confirm image is covering edge pixel or not.
					// if alpha value is 0 image is not covering, hence subsampled.
					return ctx.getImageData(0, 0, 1, 1).data[3] === 0;
				} else {
					return false;
				}
			}

			/**
			 * Detecting vertical squash in loaded image.
			 * Fixes a bug which squash image vertically while drawing into canvas for some images.
			 */
			function detectVerticalSquash(img, iw, ih) {
				var canvas = document.createElement('canvas');
				canvas.getContext("2d").globalCompositeOperation = 'source-atop';
				canvas.width = 1;
				canvas.height = ih;
				var ctx = canvas.getContext('2d');
				ctx.drawImage(img, 0, 0);
				var data = ctx.getImageData(0, 0, 1, ih).data;
				// search image edge pixel position in case it is squashed vertically.
				var sy = 0;
				var ey = ih;
				var py = ih;
				while (py > sy) {
					var alpha = data[(py - 1) * 4 + 3];
					if (alpha === 0) {
						ey = py;
					} else {
						sy = py;
					}
					py = (ey + sy) >> 1;
				}
				var ratio = (py / ih);
				return (ratio === 0) ? 1 : ratio;
			}

			/**
			 * Rendering image element (with resizing) and get its data URL
			 */
			function renderImageToDataURL(img, options, doSquash) {
				var canvas = document.createElement('canvas');
				renderImageToCanvas(img, canvas, options, doSquash);
				return canvas.toDataURL("image/jpeg", options.quality || 0.8);
			}

			/**
			 * Rendering image element (with resizing) into the canvas element
			 */
			function renderImageToCanvas(img, canvas, options, doSquash) {
				//canvas.getContext("2d").globalCompositeOperation = 'source-atop';
				var iw = img.naturalWidth, ih = img.naturalHeight;
				if (!(iw + ih)) return;
				var width = options.width, height = options.height;
				var ctx = canvas.getContext('2d');
				ctx.save();
				transformCoordinate(canvas, ctx, width, height, options.orientation);
				var subsampled = detectSubsampling(img);
				if (subsampled) {
					iw /= 2;
					ih /= 2;
				}
				var d = 1024; // size of tiling canvas
				var tmpCanvas = document.createElement('canvas');
				tmpCanvas.width = tmpCanvas.height = d;
				var tmpCtx = tmpCanvas.getContext('2d');
				//tmpCtx.globalCompositeOperation = 'source-atop';
				var vertSquashRatio = doSquash ? detectVerticalSquash(img, iw, ih) : 1;
				var dw = Math.ceil(d * width / iw);
				var dh = Math.ceil(d * height / ih / vertSquashRatio);
				var sy = 0;
				var dy = 0;
				while (sy < ih) {
					var sx = 0;
					var dx = 0;
					while (sx < iw) {
						tmpCtx.clearRect(0, 0, d, d);
						tmpCtx.drawImage(img, -sx, -sy);
						ctx.drawImage(tmpCanvas, 0, 0, d, d, dx, dy, dw, dh);
						sx += d;
						dx += dw;
					}
					sy += d;
					dy += dh;
				}
				ctx.restore();
				tmpCanvas = tmpCtx = null;
			}

			/**
			 * Transform canvas coordination according to specified frame size and orientation
			 * Orientation value is from EXIF tag
			 */
			function transformCoordinate(canvas, ctx, width, height, orientation) {
				switch (orientation) {
					case 5:
					case 6:
					case 7:
					case 8:
						canvas.width = height;
						canvas.height = width;
						break;
					default:
						canvas.width = width;
						canvas.height = height;
				}
				switch (orientation) {
					case 2:
						// horizontal flip
						ctx.translate(width, 0);
						ctx.scale(-1, 1);
						break;
					case 3:
						// 180 rotate left
						ctx.translate(width, height);
						ctx.rotate(Math.PI);
						break;
					case 4:
						// vertical flip
						ctx.translate(0, height);
						ctx.scale(1, -1);
						break;
					case 5:
						// vertical flip + 90 rotate right
						ctx.rotate(0.5 * Math.PI);
						ctx.scale(1, -1);
						break;
					case 6:
						// 90 rotate right
						ctx.rotate(0.5 * Math.PI);
						ctx.translate(0, -height);
						break;
					case 7:
						// horizontal flip + 90 rotate right
						ctx.rotate(0.5 * Math.PI);
						ctx.translate(width, -height);
						ctx.scale(-1, 1);
						break;
					case 8:
						// 90 rotate left
						ctx.rotate(-0.5 * Math.PI);
						ctx.translate(-width, 0);
						break;
					default:
						break;
				}
			}

			var URL = window.URL && window.URL.createObjectURL ? window.URL :
					  window.webkitURL && window.webkitURL.createObjectURL ? window.webkitURL :

					  null;

			/**
			 * MegaPixImage class
			 */
			MegaPixImage = function (srcImage) {
				if (window.Blob && srcImage instanceof Blob) {
					if (!URL) { throw Error("No createObjectURL function found to create blob url"); }
					var img = new Image();
					img.src = URL.createObjectURL(srcImage);
					this.blob = srcImage;
					srcImage = img;
				}
				if (!srcImage.naturalWidth && !srcImage.naturalHeight) {
					var _this = this;
					srcImage.onload = srcImage.onerror = function () {
						var listeners = _this.imageLoadListeners;
						if (listeners) {
							_this.imageLoadListeners = null;
							for (var i = 0, len = listeners.length; i < len; i++) {
								listeners[i]();
							}
						}
					};
					this.imageLoadListeners = [];
				}
				this.srcImage = srcImage;
			}

			/**
			 * Rendering megapix image into specified target element
			 */
			MegaPixImage.prototype.render = function (target, options, callback) {
				if (this.imageLoadListeners) {
					var _this = this;
					this.imageLoadListeners.push(function () { _this.render(target, options, callback); });
					return;
				}
				options = options || {};
				var imgWidth = this.srcImage.naturalWidth, imgHeight = this.srcImage.naturalHeight,
					width = options.width, height = options.height,
					maxWidth = options.maxWidth, maxHeight = options.maxHeight,
					doSquash = !this.blob || this.blob.type === 'image/jpeg';
				if (width && !height) {
					height = (imgHeight * width / imgWidth) << 0;
				} else if (height && !width) {
					width = (imgWidth * height / imgHeight) << 0;
				} else {
					width = imgWidth;
					height = imgHeight;
				}
				if (maxWidth && width > maxWidth) {
					width = maxWidth;
					height = (imgHeight * width / imgWidth) << 0;
				}
				if (maxHeight && height > maxHeight) {
					height = maxHeight;
					width = (imgWidth * height / imgHeight) << 0;
				}
				var opt = { width: width, height: height };
				for (var k in options) opt[k] = options[k];

				var tagName = target.tagName.toLowerCase();

				if (tagName === 'img') {
					target.src = renderImageToDataURL(this.srcImage, opt, doSquash);
				} else if (tagName === 'canvas') {
					renderImageToCanvas(this.srcImage, target, opt, doSquash);
				}

				if (typeof this.onrender === 'function') {
					this.onrender(target);
				}
				if (callback) {
					callback(this);
				}
				if (this.blob) {
					this.blob = null;
					URL.revokeObjectURL(this.srcImage.src);
				}
			};

			/**
			 * Export class to global
			 */
			if (typeof define === 'function' && define.amd) {
				define([], function () { return MegaPixImage; }); // for AMD loader
			} else if (typeof exports === 'object') {
				module.exports = MegaPixImage; // for CommonJS
			} else {
				this.MegaPixImage = MegaPixImage;
			}

		}

	},
	//阻止冒泡事件
	stopEvent: function (e) {
		if (e && e.stopPropagation) {
			e.stopPropagation();
		}
		else {
			window.event.cancelBubble = true;
		}
		if (e && e.preventDefault) {
			e.preventDefault()
		} else {
			window.event.returnValue = false;
		}
	},

	//上传图片
	myImageUpload: function ($target, path, noDatePath, fun) {

		var avatarInput = $target.find(".fileInput");

		if (!path) {
			path = "";
		}

		if (avatarInput.val() == "") {
			return;
		}
		var files = avatarInput[0].files;

		var data = new FormData();

		if (files) {
			data.append('file', files[0]);

		} else {
			alert("无上传")
			return;
		}
		//不生成日期路径
		if (noDatePath == true) {
			data.append('withoutDatePath', true, function (data) {
				//myAjaxF7("")
				$target.css({ "background-image": "url(" + data[0] + ")" })
				$target.find(".valueInput").val(data[0]);
			});
		}

		$.ajax({
			cache: false,
			type: 'post',
			dataType: 'json',
			url: '/upload/Image?path=' + path,
			data: data,
			contentType: false,
			processData: false,
			success: function (json) {

				if (json.code == 0) {

					if (fun) {
						fun(json.data);
					} else {
						$target.css({ "background-image": "url(" + json.data[0] + ")" })
						$target.find(".valueInput").val(json.data[0]);
					}

				} else {
					alert("上传出错:" + json.data);
				}
			},
			error: function (e) {
				alert(e);
			}
		});

		function transfApiUrl(urls) {
			for (var i = 0; i < urls.length; i++) {
				urls[i] = urls[i];
			}
			return urls;
		}
	},
	//myAjaxF7 通用ajax $btn：加载事件的按钮
	myAjaxF7:function(url, data, successFun, errorFun, $btn) {
    if ($btn == undefined && !(errorFun instanceof Function)) {
    		$btn = errorFun;
    		errorFun = undefined;
		}
		if ($btn instanceof Object) {
			if ($btn.hasClass('active')) { return; }
			$btn.addClass('active');
		}
		$.post(url, data, function (json) {
			if ($btn) {
				if ($btn.attr("id") == "moreBtn") $btn.show();
				$btn.removeClass('active');
			}
			json = eval("(" + json + ")");
			if (json.code === 0) {
				if (successFun) {
					successFun(json.data);
				}
			} else {
				if (json.action == "relogin") {//or code:101
					$.loginScreen();
				} else if(json.action =="attend") {
					self.location.href = json.data;
				}else{
					if (errorFun instanceof Function) {
						errorFun(json.data);
					} else {
						$.alert(json.data);
					}
				
				}
			
			}
		})
	},

	actionPageMenu: function (title, IDKeyData, clickFun) {
		var menuData = [{ text: title, label: true }];
		for (var i in IDKeyData) {
			menuData.push({
				text: IDKeyData[i].Key,
				onClick: (function (id) {
					return function () {
						if (clickFun) { clickFun(id) };
					}
				})(IDKeyData[i].ID)
			})
		}
		myApp.actions([menuData, { text: '关闭' }]);
	},
	frameAjaxContent: function (url, arg, pageName, renderAfterFun) {

		myfun.myFun(url, arg, function (data) {
			var page = mainView.activePage;
			var t7 = myCache.frameGetT7(page, 0)
			$(page.container).find('.page-content').html(t7(data));
			if (renderAfterFun instanceof Function) {
				renderAfterFun();
			}
		})
	},
	//$index：处在多对儿T7 T7DOM时 触发哪一个T7 默认0
	//replace: 首次内容渲染是否是替换 默认false （再次渲染时会自动变动append追加
	//callbackFun:渲染完毕后的处理
	//renderBeforFun:渲染前的数据处理，要求retrun data;
	f7AjaxList: function (page, url, arg, pageSize, $index, replace, callbackFun, renderBeforFun) {
		//框架下Ajax请求List，并实现MoreBtn点击加载更多，改变moreBtn效果
		//参数page 为onPageinit时page事件参数
		//需要模板<script id=contentT7
		//需要渲染容器 <div id=contentT7Dom
		//需要<button id=moreBtn
		//以下为代码
		var pagenav = {
			index: 0,
			pageSize: 12,
			allLoaded: false
		}
		if (pageSize) {
			pagenav.pageSize = pageSize;
		}
		if (!replace) {
			replace = false;
		}

		if ($index == undefined) { $index = 0 };
		var contentT7 = $.t7.compile($(page.container).find("#contentT7").eq($index).html())
		var btn = $(page.container).find("#moreBtn").eq($index);

		var _btnClick = function () {

			var urlArg = JsonExtend(pagenav, arg);
			console.log("ajax:" + url);
			console.log(urlArg);
			myFun.myAjaxF7(url, urlArg, function (data) {

				//之前的数据处理
				if (renderBeforFun instanceof Function) {
					data = renderBeforFun(data);
					if (data == undefined) { alert("renderBeforFun需要return 数据"); return; }
				}

				//alert("myAjaxF7 complate");
				var html = contentT7(data);
				if (replace) {
					replace = false;
					$(page.container).find('#contentT7Dom').eq($index).html(html);
				} else {
					$(page.container).find('#contentT7Dom').eq($index).append(html);
				}
				//myApp.initImagesLazyLoad(page.container);
				if (data instanceof Array)
					if (data.length < pagenav.pageSize) {
						pagenav.allLoaded = true;
						$(page.container).find("#moreBtn").eq($index).hide();

					} else {
						pagenav.index += pagenav.pageSize;
					}
				if (callbackFun) {
					callbackFun();
				}
			}, btn)

		}



		if (!btn.data("f7AjaxListHandler")) {
			btn.on("click", _btnClick).data("f7AjaxListHandler", true);
		}
		_btnClick()
		//btn.click();
	},
	//针对点中某个tab ajax请求某一ajax，并显示对应tab-content上
	//url
	//arg url对应参数
	//tab元素，用于获取是第几个元素，对应使用第几个模板，渲染到第几个DOM
	//callbackFun回调
	tabAjaxList: function (url, arg, tab, callbackFun) {
		//alert("tabAjaxList begin");
		//当对应有内容时，不在请求新数据
		var $index = 0;
		try {
			$index = $(tab).index();
		} catch (e) { };


		myFun.f7AjaxList(mainView.activePage, url, arg, 12, $index, true, callbackFun);
		//滑动
		//	myApp.swipeoutOpen(d.children().eq(0), "right");

	}
}


//loaded Event
$(function () {
	var $page = $(".page");
	ITC._pageInitAction(null, $page.get(0).id, $page);
})
//pageInit Event
$(document).on("pageInit", function (e, pageId, $page) {
	ITC._pageInitAction(e, pageId, $page);
});
$(document).on("pageReinit", function (e, pageId, $page) {
    
});

//增强控制中心
var ITC = {
	//page出始化处理
	_pageInitAction: function (e, pageId, $page) {
		//处理页面的方法
		if (ITC.pageInitActions[pageId]) {
			ITC.pageInitActions[pageId](e, pageId, $page);
		}
		//处理页面的f7 ajax
		var f7Doms = $page.find(".f7AjaxList");
		f7Doms.each(function (i, v) {
			if (!$(this).data("auto")) { return; }

			var page = {container:$page,name:pageId}
			var url = $(this).data("url");
			var pageSize =  parseInt($(this).data("size"),10);
			var arg = eval($(this).data("arg"));
			myFun.f7AjaxList(page, url, arg, pageSize, i);

		})
		
	},
	//page出始化库
	pageInitActions : {
		material: function (e, pageId, $page) {
			$.get("/content/Materialjson/" + $page.data("materilId") + ".json", function (json) {
				TMFrame.init(json)
			})
		},
		main: function (e, pageId, $page) {
			homeAction()
		},
		testStart: function (e, pageId, $page) {
			Test.timer(parseInt($page.data("time"), 10));
		},
		voteList: function (e, pageId, $page) {
			$page.find("header").removeClass("nav-nice2-active");
			setTimeout(function () {
				$page.find("header").addClass("nav-nice2-active");
			}, 200)
			setTimeout(function () {
				$(".vote-chart").each(function (i, v) {
					if (!$(this).data("data")) return;
					var data = $(this).data("data").split("|");

					d3RowBarChart(this, eval("([" + data[1] + "])"), 400, 300);
				})
				//显示chart
				var chart = $(".vote-chart svg");
				var charti = 0;
				setTimeout(function () {
					chart.eq(charti).css("height:", chart.eq(charti).attr("height") + "px");
					charti++;
					setTimeout(arguments.callee.caller, 200);
				}, 1000)

			}, 500)



		},
		voteDetial: function () {
			console.log("ProtonsFrame.init");
			ProtonsFrame.init();

			setTimeout(function(){
			var pt = $(".vote-title");
			var str = pt.find("span");
			var strI = 0;
			var isNext = false;
			var oldTimestamp = 0;
			var timeLimit = 30;
			var handle = requestAnimationFrame(function (timestamp) {
				if (timestamp - oldTimestamp < timeLimit) {
					requestAnimationFrame(arguments.callee);
					return;
				};

				oldTimestamp = timestamp;
				str.eq(strI).addClass("s")
				//pt.append("<span>" + str[strI] + "</span>");
				strI++;
				if (str.length == strI) { return; }
				requestAnimationFrame(arguments.callee);

				
			});
			

			},500)
		}
	},
	pageAction: {
		//选择投票项
		selectVoteItme: function (it) {
			
			var body = $($.router.getCurrentPage());
			var isNew = body.hasClass("isNew");
			if (!isNew) { return;}
			body.data("selectid", $(it).data("id"));

			$(it).siblings().removeClass("active")
			body.removeClass("shake");

			setTimeout(function () {
				body.addClass("shake");
				$(it).addClass("active")
			}, 20)

			var targetOffset = $(it).offset();

			ProtonsFrame.One(targetOffset.left, targetOffset.top);
		},
		//提交投票项
		submitVote: function () {
			var body = $($.router.getCurrentPage());
			var voteItemId = body.data("selectid");
			var voteId = body.data("vote-id");
			if (isNaN(voteItemId)) {
				body.toggleClass("submit-alert");
				$.alert("请先选择你的投票项", function () {
					body.toggleClass("submit-alert");
				});
				return;
			}
			myAjaxPost("/vote/submit", {voteId:voteId, voteItemId: voteItemId }, function (json) {
				body.toggleClass("submit-alert");
				$.alert("投票成功", function () {
					body.removeClass("isNew");
					body.toggleClass("submit-alert");

					$.router.loadPage({
						url: "/vote/detial/" + voteId,
						noAnimation: true,
						replace: true
					});

				});
			}, function (msg) {
				body.toggleClass("submit-alert");
				$.alert(msg, function () {
					body.toggleClass("submit-alert");
				});
			})
		},
		//绑定工号
		bind: function () {
			var workid = $("#input_workId").val();
			var password = $("#input_password").val();
			myAjaxPost("/my/Bind", { workid: workid, password: password }, function (res) {
				$.alert(res + "，返回个人中心", function () {
					$.router.loadPage({
						url: "/my",
						noAnimation: false,
						replace: true
					});
				})
			},
			function (err) {
				$.alert(err);
			})
		},
		//解绑工号
		unBind: function () {
			myAjaxPost("/my/unBind", {confirm:true }, function (res) {
				$.alert("解绑成功,返回我的中心", function () {
					$.router.loadPage("/my/index")
				})
			},
			function (err) {
				alert(err);
			})
		}

	}
	
}




var _home_o_timer;
function homeAction() {

    var _home_o = $("#signScroller");
    var _home_n = _home_o.children().length;
    var _home_h = _home_o.children().eq(0).height();
    var _home_i = 0;
    if (_home_o_timer) {
        clearTimeout(_home_o_timer);
    }
    _home_o_timer = setTimeout(function () {


        if (!_home_o.parent()) { return; }

        if (_home_i == _home_n) { _home_i = 0; }

        _home_o.css({ transform: "translateY(-" + _home_i * _home_h + "px)" });

        _home_i++;

        _home_o_timer = setTimeout(arguments.callee, 5000);

        }, 5000)

}
