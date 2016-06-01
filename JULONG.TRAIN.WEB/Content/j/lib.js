/**********************/
//json to url参数;
var JsonToUrlParam = function (jsonParam) {
	var str = new String();
	var i = 0;
	for (var p in jsonParam) {

		if (!jsonParam[p]) {
			continue;
		}


		if (i == 0) {
			str += "?";
		} else {
			str += "&";
		}
		str += p + "=" + jsonParam[p]
		i++;
	}
	return str;

};
var GetUrlParam = function (name) {
	var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
	var r = window.location.search.substr(1).match(reg);
	if (r != null) return decodeURI(r[2]); return {};
}
//url参数toJson
var UrlParamToJson = function (url) {

	var obj = {};
	var keyvalue = [];
	var key = "", value = "";
	var paraString = url.substring(url.indexOf("?") + 1, url.length).split("&");
	for (var i in paraString) {
		keyvalue = paraString[i].split("=");
		key = keyvalue[0];
		value = keyvalue[1];
		obj[key] = value;
	}
	return obj;
}
//浅表合并Json(Object)
var JsonExtend = function (target, obj) {
	if (!obj) { return target };
	for (var i in obj) {
		target[i] = obj[i];
	}
	return target;
};
/**********************/

//时间段
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
	//计时
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
		if (typeof (json) == "string") { json = eval("(" + json + ")") };
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
			} if (json.action == "relogin") {
				alert("需要登录");
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



//angularjs项目组件注入
var ngInit = function ($scope, $http, $upload) {
	$scope.siteConfig = siteConfig;
	$scope.safeApply = function (fn) {
		var phase = this.$root.$$phase;
		if (phase == '$apply' || phase == '$digest') {
			if (fn && (typeof (fn) === 'function')) {
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
				if (imgUploadEl.length != 0) {
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

//d3.js 横向柱形图 windbell2
function d3RowBarChart(target, data, width, height,total) {
	if (!d3) {
		console.log("lose d3.js")
		return;
	}
	if (!data instanceof Array) {
		console.log("data not array")
		return;
	}
	var colors = ["#f29b7", "#f6b37f", "#facd89", "#fff799", "#cce198", "#acd598","#89c997","#84ccc9","#7ecef4","#88abda","#8c97cb]"]
	var barHeight = 14;
	var baseY = 14 + 4;
	var _y = 5;
	var fontSize = 12;

	var index = "ABCDEFGHIJK"
	width = width || 400,
	height = data.length*baseY+_y || 300

	var maxBarWidth = width - 50;

	var svg = d3.select(target)
		.append("svg")
		.attr("width", width)
		.attr("height", height)

	svg.append("rect").attr("width", 1).attr("height", data.length * 19).attr("x", 20).attr("y", 0).attr("fill", "rgb(191, 184, 104)")


	var g = svg.selectAll("g")
		.data(data)
		.enter()
		.append("g")
	
	g.append("rect")
		.attr("x", 20)
		.attr("y", function (d, i) {
			return i * baseY+_y;
		})
		.attr("height", barHeight)
		.attr("width", function (d, i) {
			if (total) {
				return d / total * 350;
			} else {
				var v = d * 5;
				return v > maxBarWidth ? maxBarWidth : v;
			}

		})
		.attr("fill", function (d, i) {
			//var c = colors[i+3] || "#ff9900";
			return "#bfb868";
		});
	
	g.append("text")
		.attr("font-size", fontSize)
		.attr("x", function (d, i) {
			if (total) {
				return d / total * 350 +25;
			} else {
				var v = d * 5;
				return v > maxBarWidth ? maxBarWidth + 25 : v + 25;
			}

		})
		.attr("y", function (d, i) {
			return i * baseY + _y + fontSize;
		})
		.attr("fill", "#6f8492")
		.text(function (d, i) {
			return d;
		})

	g.append("text")
		.attr("font-size", fontSize)
		.attr("x", 5)
		.attr("y", function (d, i) {
			return i * baseY + _y + fontSize;
		})
		.attr("fill", "rgb(191, 184, 104)")
		.text(function (d, i) {
			return index[i];
		})


}

//粒子喷射


var ProtonsFrame = {
	init: function () {
		//粒子效果
		document.addEventListener("myProtonAnimationEnd", function (e) {
			if(ProtonsFrame.Protons.length==0 )return;
			document.body.removeChild(ProtonsFrame.Protons[0].c);
			ProtonsFrame.Protons.splice(0, 1);

		})

		//帧动画另一种写法


		var lastTime = 0;
		var vendors = ['ms', 'moz', 'webkit', 'o'];
		for (var x = 0; x < vendors.length && !window.requestAnimationFrame; ++x) {
			window.requestAnimationFrame = window[vendors[x] + 'RequestAnimationFrame'];
			window.cancelAnimationFrame = window[vendors[x] + 'CancelAnimationFrame'] || window[vendors[x] + 'CancelRequestAnimationFrame'];
		};

		if (!window.requestAnimationFrame) {
			window.requestAnimationFrame = function (callback, element) {
				var currTime = new Date().getTime();
				var timeToCall = Math.max(0, 16 - (currTime - lastTime));
				var id = window.setTimeout(function () { callback(currTime + timeToCall); }, timeToCall);
				lastTime = currTime + timeToCall;
				return id;
			};
		};

		if (!window.cancelAnimationFrame) {
			window.cancelAnimationFrame = function (id) {
				clearTimeout(id);
			};
		};

	},
	hueStart: 0,
	hueEnd: 220,
	hue: 0,
	Protons: [],
	One: function (x, y) {
		var canvas = document.createElement("canvas");
		canvas.width = 300;
		canvas.height = 200;
		canvas.className = "particle-canvas";
		canvas.style.top = y - canvas.height / 2 + 18 + "px";
		canvas.style.left = x - canvas.width / 2 +20+ "px";
		document.body.appendChild(canvas);
		var p = new myProton(canvas);
		p.hue = this.hue;
		//if (_this.hue == _this.hueEnd) { _this.hue = _this.hueStart };
		this.Protons.push(p);
		p.star();

	}
}

//粒子喷射对象 基一个loader.js 提取其粒子实现部分，并做了修改
//c:Canvas
var myProton = function (c) {
	this.pool = [];
	if (!c) {
		console.log("lost canvas");
	}
	var _this = this;
	this.c = c;
	this.ctx = c.getContext('2d');
	this.cw = c.offsetWidth;
	this.ch = c.offsetHeight;

	this.ParticleXY = {
		x: (this.cw / 2),
		y: (this.ch / 2)
	};

	this.particles = [];
	this.particleLift = 140;
	this.hueStart = 0
	this.hueEnd = 220;
	this.hue = 0;
	this.gravity = .15;
	this.particleRate = 4;
	this.particleNumber = 120;
	this.isEnd = false;


	this.rand = function (rMi, rMa) { return ~~((Math.random() * (rMa - rMi + 1)) + rMi); };
	this.hitTest = function (x1, y1, w1, h1, x2, y2, w2, h2) { return !(x1 + w1 < x2 || x2 + w2 < x1 || y1 + h1 < y2 || y2 + h2 < y1); };


	this.star = function () {
		var loopIt = function () {
			_this.clearCanvas();
			if (_this.particleNumber > 1) {
				_this.createParticles();

			} else {
				if (_this.isEnd) {

					var evt = document.createEvent("Events");
					console.log("myProtonAnimationEnd")
					evt.initEvent('myProtonAnimationEnd', true, true);
					document.dispatchEvent(evt);
					return;
				}
			}
			requestAnimationFrame(loopIt, _this.c);
			//_this.updateLoader();
			_this.updateParticles();

			//_this.renderLoader();
			_this.renderParticles();

		};
		loopIt();
	}

	this.clearCanvas = function () {
		this.ctx.globalCompositeOperation = 'source-over';
		this.ctx.clearRect(0, 0, this.cw, this.ch);
		this.ctx.globalCompositeOperation = 'lighter';
	};

	this.Particle = function () {
		this.live = 100;
		this.x = _this.ParticleXY.x + _this.rand(-12, 12);
		this.y = _this.ParticleXY.y + _this.rand(-7, 7);
		this.vx = (_this.rand(-220, 220) - 2) / 100;
		this.vy = (_this.rand(0, _this.particleLift) - _this.particleLift * 2) / 100;
		this.width = _this.rand(2, 8) / 2;
		this.height = _this.rand(2, 8) / 2;
		this.hue = ProtonsFrame.hue++;
		if (ProtonsFrame.hue == ProtonsFrame.hueEnd) { ProtonsFrame.hue = ProtonsFrame.hueStart };
		_this.particleNumber--;

	};

	this.Particle.prototype.update = function (i) {
		this.vx += (_this.rand(0, 6) - 3) / 100;
		this.vy += _this.gravity;
		this.x += this.vx;
		this.y += this.vy;

		if (this.live < 0) {
			_this.particles.splice(i, 1);
			if (_this.particles.length == 0) {
				_this.isEnd = true;
			}
		}

		//if (this.y > _this.ch) {
		//	_this.particles.splice(i, 1);
		//}
	};

	this.Particle.prototype.render = function () {
		this.live -= 2;
		_this.ctx.fillStyle = 'hsla(' + this.hue + ', 100%, ' + _this.rand(50, 70) + '%, ' + (this.live) / 100 + ')';
		_this.ctx.fillRect(this.x, this.y, this.width, this.height);
	};

	this.createParticles = function () {
		var i = this.particleRate;
		while (i--) {
			this.particles.push(new this.Particle());
		};
	};

	this.updateParticles = function () {
		var i = this.particles.length;
		while (i--) {
			var p = this.particles[i];
			p.update(i);
		};
	};

	this.renderParticles = function () {

		var i = this.particles.length;
		while (i--) {
			var p = this.particles[i];
			p.render();
		};
	};
}

//帧动画驱动
window.requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame || window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;
