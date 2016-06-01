/*
windbell2 lib for ng 1.4.8
2016.1.14
*/
(function (window, angular, undefinded) {
	'use strict';
	var $ngW2libMinErr = angular.$$minErr('ngW2lib');
	var modual = angular.module("ngW2lib", [])
		//库增加服务
		.service('w2lib', function ($http, $q) {//provider 方式也可

			function ajax(url, data, method) {

				var deferer = $q.defer();
				var promise = deferer.promise;
				promise.success = function (fun) {
					promise.then(fun)
					return promise;
				}
				promise.error = function (fun) {
					if(fun){
						promise.catch(fun);
					} else {//缺省处理
						promise.catch(function (data) { alert(data) });
					}
					return promise;
				}


				$http({
					url: url,
					data: data,
					method: method,
					headers: {
						//'Content-Type': 'application/x-www-form-urlencoded',
						//'Accept': "*/*",
						'By': "w2LibRequest"
					}
				}).then(function (res) {

					if (!("code" in res.data)) {
						var err = "返回格式非myJson" + "\n" + res.data;
							deferer.reject(err)
						console.log(err)
						return;
					}
					if (res.data.code === 0) {
						deferer.resolve(res.data.data)
					} else {
						_filters.forEach(function (v) {
							if (v.action == res.data.action) {
								return (v.filter(res.data));
							}
						})

						deferer.reject(res.data.data);

						console.log(res.data.data);

					}
				}, function (res) {
					deferer.reject(res.status + "\n" + res.statusText)
					console.log("http错误:" + res.status + "\n" + res.statusText)
				});
				return promise;
				/* 传统方式
				var p = {
					_successFun:null,
					success: function (fun) {
						p._successFun = fun;
						return res;
					},
					_errorFun:null,
					error: function (fun) {
						p._errorFun = fun;
						return res;
					}

				}

				$http({
					url:url,
					data:data,
					method: type,
					headers: {
						'Content-Type': 'application/x-www-form-urlencoded'
					}
				}).then(function (res) {
					if (res.code == 0) {
						p._successFun(res.data)
					} else {
						if (p._errorFun) {
							p._errorFun(res.data);
						}
						
					}
				}, function (res) {
					if (p._errorFun) {
						p._errorFun(res.data);
					}
				})

				return p;
				*/
			};
			/*defer promise异步方式实现*/
			this.ajax = {
				get: function (url, data) {
					return ajax(url, data, "GET");
				},
				post: function (url, data) {
					return ajax(url, data, "POST")
				}
			};
			var _filters = []
			//结果处理注入
			this.ajax.filterInjector = function (action, fun) {
				_filters.push({ action: action, filter: fun })
			}


			//封装$http 实现解释myJson{code:0,data:{}}格式
			//思考:使用拦截器 "$httpProvider.interceptors 可以更粗暴的拦截解析
			/* 例子：增加loading菊花

<div class="loading-modal modal" ng-if="loading">
    <div class="loading">
        <img src="<?=$this->module->getAssetsUrl()?>/img/loading.gif" alt=""/><span ng-bind="loading_text"></span>
    </div>
</div>


app.config(["$routeProvider", "$httpProvider", function ($routeProvider, $httpProvider) {
    $routeProvider.when('/', {
        templateUrl: "/views/reminder/index.html",
        controller: "IndexController"
    });
    $routeProvider.when('/create', {
        templateUrl: "/views/reminder/item/create.html",
        controller: "ItemCreateController"
    });
    $routeProvider.otherwise({redirectTo: '/'});
    $httpProvider.interceptors.push('timestampMarker');
}]);
 
//loading
app.factory('timestampMarker', ["$rootScope", function ($rootScope) {
    var timestampMarker = {
        request: function (config) {
            $rootScope.loading = true;
            config.requestTimestamp = new Date().getTime();
            return config;
        },
        response: function (response) {
           // $rootScope.loading = false;
            response.config.responseTimestamp = new Date().getTime();
            return response;
        }
    };
    return timestampMarker;
}]);
			*/

		})
		//日期指令
		.directive("ngW2datepicker",  function ($timeout, $parse, $filter) {
		
			return {
				restrict: "A",
				require: 'ngModel',
				template: "",
				replace: false,
				scope: {
					ngModel:"=",
					ngW2datepickerFormat:"@"
				},
				link: function (scope, ele, attr,ctl) {
					//得到ngModel句柄
					var ngModelGetter = $parse(attr["ngModel"]);

					var format = scope.ngW2datepickerFormat || "YYYY-MM-DD";

					var _ele = $(ele).datetimepicker({ format: format })
					var _tar = _ele.children()[0];
					scope.$watch('ngModel', (function (newValue, oldValue) {
						if (newValue) {
							_tar.value = $filter('date')(newValue, 'yyyy-MM-dd')
						}
						//return function (newValue, oldValue) {
						//_tar.value= newValue;
						//tar.value = newValue;
					}
					//})
					//()
					)
					);

					_ele.on('dp.change', function (evt) {
						
						//var tar = evt.target.tagName == "INPUT" ? evt.target : evt.target.children[0];

						scope.ngModel = _tar.value;
							
						scope.$apply(function () {
							//更新
							return ngModelGetter.assign(scope, tar.value);
							
						});
					});
				}
			};

		})



})(window, window.angular);

