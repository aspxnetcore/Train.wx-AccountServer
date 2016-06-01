function TimeSpan(h, m, s) {
	this.set(h, m, s);
}

TimeSpan.prototype = {
	set: function (h, m, s) {
		this.h = Math.round(Number(h));
		this.m = Math.round(Number(m));
		this.s = Math.round(Number(s));

		this.m += Math.floor(this.s / 60);
		this.s = this.s % 60;
		this.h += Math.floor(this.m / 60);
		this.m = this.m % 60;

		this.h = this.h % 24;
		return this;
	},
	toString: function () {
		return (this.h < 10 ? "0" + this.h.toString() : this.h.toString())
			+ ":" + (this.m < 10 ? "0" + this.m.toString() : this.m.toString())
			+ ":" + (this.s < 10 ? "0" + this.s.toString() : this.s.toString());
	},
	timer: null,

	stop: function () {

	},

	callback: Function(),

	start: function (callback) {
		var self = this;

		if (callback) {

			this.callback = callback;
		}

		if (isNaN(this.s)) {
			return;
		}

		self.timer = setInterval(function () {

			self.s--;

			if (self.s >= 0) {
				self.callback();
				return;

			}

			//s < 0, m > 0
			if (self.m > 0) {
				self.s = 59;
				self.m--;
				self.callback();
				return;
			}

			//s < 0 ,m = 0, h<1
			if (isNaN(self.h) || self.h < 1) {
				//self.callback();
				clearInterval(self.timer);
				return;
			}


			self.m = 59;
			self.s = 59;
			self.h--
			self.callback();
		}, 1000);

	}
};

//isCenter 在中间显示
function myAlert(title, content, isCenter) {
	$.gritter.add({
		title: title,
		text: content,
		image: '/Content/manage/avatars/avatar3.png',
		sticky: false,
		time: 600,
		class_name: isCenter != true ? 'gritter-info gritter-center' : ""

	});
}

//移出html
String.prototype.removeHTML = function () {
	return this.replace(/<\/?[^>]*>/g, '');
}
function removeHTML(str) {
	return str.replace(/<\/?[^>]*>/g, '');
}
//数组对象定义一个方法，用于查找指定的元素在数组中的位置（索引）
Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
        }
    return -1;
        };

        	//数组对象定义一个方法,通过元素的索引删除元素
Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
    };

//ajax处理
//successFun(data) data:成功返回的数据
//errorFun(data) data:失败的返回的数据(错误提示)
//在fun中配合$form.serializeReverseForm(data) [反序列号],可将ajax返回结果直接映射到表单对应元素中
function myAjaxPost(url, data, successFun, errorFun) {
	$.post(url, data, function (json) {
		if ("code" in json) {
			if (json.code == 0) {
				if ($.isFunction(successFun)) {
					successFun(json.data);
				}

			} else {
				if ($.isFunction(errorFun)) {
					errorFun(json.data);
				} else {
					alert(json.data)
				}
			}
			if (json.action == "reload") {
				self.location.reload();
			}
		} else {
			alert("结果不是myJson格式")
		}
	});
}

//反序列化 form
$.fn.serializeReverseForm = function (json) {

	for (var _field in json) {
		var _target = this.find("[name=" + _field + "]")

		if (_target.length == 0) { continue; }
		if (_target.get(0).tagName == "TEXTAREA") {
			if (_target.data("toggle") == "redactor") {
				_target.prev().html(json[_field])
				_target.val(json[_field])
				_target.html(json[_field])
			} else {
				_target.html(json[_field])
			}

			continue;
		}

		if (_target.attr("type") == "checkbox") {

			if (json[_field] === true || json[_field] === false || json[_field] === "false" || json[_field] === "true") {
				_target.prop("checked", json[_field])
			}
			else {
				alert(_field + "不是bool类型,可能是多个值的数组，暂时不支持它的反序列化，有需要再完善");
			}
			continue;
		}
		if (_target.attr("type") == "raido") {
			_target.each(function () {

				if ($(this).val() == json[_field].toString()) {
					$(this).prop("checked", true);
				}
			})
			continue;
		}
		else {
			_target.val(json[_field])
		}

	}
}
//序列化to Object 

$.fn.serializeObject = function () {

	var o = {};

	var a = this.serializeArray();

	$.each(a, function () {

		if (o[this.name]) {

			if (!o[this.name].push) {

				o[this.name] = [o[this.name]];

			}

			o[this.name].push(this.value || '');

		} else {

			o[this.name] = this.value || '';

		}

	});

	return o;

};
//解析后台转出的时间JSON "/Date(1397491200000)/"
function FormatTime(jsonDate, format) {
	var _jsonDate = jsonDate.split('(')[1].split(')')[0];
	var rDate = new Date(parseInt(_jsonDate));
	return rDate.pattern(format);
}
Date.prototype.pattern = function (fmt) {
	var o = {
		"M+": this.getMonth() + 1, //月份      
		"d+": this.getDate(), //日      
		"h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时      
		"H+": this.getHours(), //小时      
		"m+": this.getMinutes(), //分      
		"s+": this.getSeconds(), //秒      
		"q+": Math.floor((this.getMonth() + 3) / 3), //季度      
		"S": this.getMilliseconds() //毫秒      
	};
	var week = {
		"0": "\u65e5",
		"1": "\u4e00",
		"2": "\u4e8c",
		"3": "\u4e09",
		"4": "\u56db",
		"5": "\u4e94",
		"6": "\u516d"
	};
	if (/(y+)/.test(fmt)) {
		fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
	}
	if (/(E+)/.test(fmt)) {
		fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "\u661f\u671f" : "\u5468") : "") + week[this.getDay() + ""]);
	}
	for (var k in o) {
		if (new RegExp("(" + k + ")").test(fmt)) {
			fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
		}
	}
	return fmt;
}

//以下应用于 Ace(bootstrap) 模板

//激活菜单
function activeLeftMenu(ViewBag_menuUrl) {
	var _href = (ViewBag_menuUrl || self.location.href).toLowerCase();
	//中文
	if (_href.indexOf("%") < 0) { _href = encodeURI(_href).toLowerCase(); }

	var _as = document.getElementById("page_nav").getElementsByTagName("a");
	for (var i = 0; i < _as.length; i++) {
		if (_href.indexOf(encodeURI(_as[i].attributes["href"].value.toLowerCase()).toLowerCase()) >= 0) {
			_as[i].className = "active";
			var _parent = _as[i].parentNode;
			if (_parent.tagName.toLowerCase() == "li") {
				_parent.className = "open active";
				var __parent = _parent.parentNode.parentNode;
				if (__parent.tagName.toLowerCase() == "li") {
					__parent.className = "open active";
					//三级上返

					__parent = __parent.parentNode.parentNode;
					if (__parent.tagName.toLowerCase() == "li") {
						__parent.className = "open active";
					}
					return;
				}
				return;
			}
			return;
		}
	}
}
var siteConfig = {
	ResBasePath: "",
	UploaResdBasePath: "/manage/upload/ImageUpload"
}

//angularjs项目组件注入
var ngInit = function ($scope, $http, $upload) {
	$scope.siteConfig = siteConfig;
	$scope.safeApply = function(fn) {
		var phase = this.$root.$$phase;
		if (phase == '$apply' || phase == '$digest') {	
			if (fn && (typeof(fn) === 'function')) {
				fn();
			}
		} else {
			this.$apply(fn);
		}
	};
	$scope.myAjax = function ($httpFun, successFun) {
		$httpFun.success(function (res) {
			if (res.code == 0) {
				successFun(res.data)
			} else {
				alert(res.data);
			}

		}).error(function (res) {
			alert(res);
		})
	}
	if ($upload) {
		$scope.upload = function (file, evt, compeleFun) {
			if (file == null) { return; }
			var it = $($.event.fix(evt).currentTarget);
			var oldFileFullName = it.prev().data("value") || "";
			$upload.upload({
				url: siteConfig.UploaResdBasePath,
				data: {
					file: file,
					fileFullName: oldFileFullName,
					path: it.data('upload-path') || "HeadPic",
					compress: true,
					MaxFileSize: 80000, //80k
					MaxImgWidth: 768
				}
			}).then(function (resp) {
				var imgUploadEl = it.parents(".imgUpload");
				if (imgUploadEl.length!=0){
				//传输正常
					var repeatModel = it.prev().attr('data-ng-repeat-model');//采用valueInput 扩展模块-----repeat中的模型指向$scope
					if (repeatModel != undefined) {
						eval("$scope." + repeatModel + "= resp.data.filelink[0]");
					} else {
						var el = it.prev().attr('ng-model');//采用valueInput中模型
						eval("$scope." + el + "= resp.data.filelink[0]");
						//$scope.safeApply();
					}

				}

				if (compeleFun) {
					//派发file，事件元素的参数
					compeleFun(resp.data.filelink[0], it);
				}
				//console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.data);
			}, function (resp) {
				//服务器错误
				//alert(resp.status);
			}, function (evt) {
				//传输进度
				//var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
				//console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
			});
		};
	}
}


panelShow.submitFun = null;
panelShow.lastTargetParent = null;
panelShow.lastTarget = null;
//弹出菜单(表单)，$target:内容dom的$，submitFun:提交事件
//默认弹出层的ID为myLayoutModel
function panelShow($target, submitFun, title) {

	if (panelShow.lastTarget != null && panelShow.lastTarget.get(0) != $target.get(0)) {
		panelShow.lastTargetParent.append(panelShow.lastTarget);
	}

	panelShow.lastTarget = $target;
	panelShow.lastTargetParent = $target.parent();
	panelShow.submitFun = submitFun;
	if (title != undefined) {
		$("#myLayoutModel").find(".modal-title").html(title);
	}

	$("#myLayoutModel").find(".modal-body").append($target)
	$("#myLayoutModel").modal({
		//keyboard: false,
		//backdrop: "static",
	})
}
function panelSubmit() {
	if ($.isFunction(panelShow.submitFun)) {
		panelShow.submitFun()
	}

}

//处理htmlContent中的具有Src的类型为Base64(DataUrl)的IMG标签，将其base64数据汇总并转化为File(Blob)s 对象集，同时将src转为{{n}} n为标签序号
//待完成
