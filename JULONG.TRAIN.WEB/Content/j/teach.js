//教材框架 for moblie
var TMFrame = {
	chapters: [],
	pages: [],
	nowPageIndex: 0,//当前页总序号
	nowChapterIndex: 0,//当前章节序号
	nowChapterPageIndex: 0,//当前页在其章节中序号
	pageNav_Headroom: null,
	imgMaxWidth:window.screen.width,
	nowPageDom: null,
	isFullView:false,
	//pageHeaderBG_height: 0,
	init: function (jsondata) {

		this.chapters = [],
		this.pages = [],
		this.nowPageIndex = 0,//当前页总序号
		this.nowChapterIndex = 0,//当前章节序号
		this.nowChapterPageIndex = 0,//当前页在其章节中序号
		this.pageNav_Headroom = null,
		this.nowPageDom = null,
		this.isFullView = false,
		$("#tmPageTitle").html("");
		$("#tmPageContent").html("");

		if (jsondata) {
			this.initJsonToDom(jsondata)
		}

		this.chapters = $("#tree li>a");
		this.initPages();
		this.initGoto();

		$("#tree>ul").click(function (evt) {
			var ele = evt.target;
			if ((ele.tagName == "LI" || ele.tagName == "A")) {
				if (ele.tagName == "LI") { ele = ele.children[0] }

				TMFrame.goto(parseInt($(ele).attr("pageIndex")));
			}
		});
		$("#tmPageContent").click(this.fullView);
	},
	initJsonToDom: function (json) {
		$("#tree").get(0).innerHTML = buildTreeNode([],json, 0).join("");
		function buildTreeNode(str, nodes, deph) {

			if (nodes instanceof Array && nodes.length > 0) {
				str.push("<ul>")

				for (var i in nodes) {
					if (nodes[i] instanceof Function) continue;
					var n = nodes[i];
					str.push('<li class="depth-' + deph + '" title="' + n.name + '">');
					str.push('<a href="#" pid="' + n.id + '" count="' + n.pageIds.length + '" pages="' + n.pageIds.join(",") + '">' + n.name);
					str.push(n.pageIds.lenght > 0 ? '<code>' + n.pageIds.length + '</code>' : '');
					str.push("</a>");
					str.push(buildTreeNode(str,n.sub, deph + 1));
				}

				str.push("</ul>");
			}
			return str

		}
	},
	initPages: function () {
		for (var i = 0; i < this.chapters.length; i++) {
			var pageIds = this.chapters.eq(i).attr("pageIndex", this.pages.length ).removeAttr("href").attr("pages").split(",");
			if (pageIds == "") {
				this.pages.push([i, null, null, null]);//chapter
			} else {
				for (var ii = 0; ii < pageIds.length; ii++) {
					this.pages.push([i, pageIds[ii], null, ii])//page
				}
			}

		}
	},
	initGoto: function (bookmark) {
		if (bookmark == "" || bookmark == undefined) {
			this.goto(0);
		} else {
			this.gotoByBookmark(bookmark);
		}
	},
	updateTreeDom: function () {
		//更新tree 样式
		if (this.nowPageDom != null) {
			$(this.nowPageDom).removeClass("focus")
		}
		this.nowPageDom = this.chapters.eq(this.nowChapterIndex).addClass("focus");
	},
	goto: function (pageIndex) {

		this.pageImgLoadListenerRemove();
		this.reset();




		if (this.getPageId(pageIndex) == null) {//节点无page
			this.updateIndex(pageIndex, this.getChapterIndex(pageIndex), this.getChapterPageIndex(pageIndex));
			this.updateTreeDom();

			this.showChapterNoPages(pageIndex);
		} else {
			if (this.getPageContent(pageIndex) == null) {//有内容未加载
				this.loadData(this, this.getPageId(pageIndex), pageIndex, true);
				return;
			} else {//有内容已加载
				this.updateIndex(pageIndex, this.getChapterIndex(pageIndex), this.getChapterPageIndex(pageIndex));
				this.updateTreeDom();
				this.showPage(pageIndex);
				var _this = this;
				setTimeout(function () {
					_this.resetNav();
				}, 300)
			}
		}


	},
	gotoByBookmark: function (loc) {
		var _loc = loc.split("|");
		var _p = null;
		var pIndex = 0;
		var pid = _loc[0];
		var pageid = _loc[1];
		if (pageid == "") { pageid = null }
		for (var i = 0; i < this.pages.length; i++) {
			if (this.chapters.eq(i).attr("pid") == pid) {
				_p = this.chapters.eq(i);
				pIndex = i;
				break;
			}
		}
		if (_p != null) {
			var ii = 0;
			for (ii = 0; ii < this.pages.length; ii++) {
				if (this.getChapterIndex(ii) == pIndex && this.getPageId(ii) == pageid) {
					this.goto(ii);
					return;
				}
			}
		}
		alert("未找到书签地址");

	},
	getChapterIndex: function (pageIndex) {
		return this.pages[pageIndex][0];
	},
	getPageContent: function (pageIndex) {
		if (this.pages[pageIndex][2] == null) {
			return this.pages[pageIndex][2]
		}
		return this.pages[pageIndex][2].Content;
	},
	setPageContent: function (pageIndex, data) {

		this.pages[pageIndex][2] = data;

	},
	getPageId: function (pageIndex) {
		return this.pages[pageIndex][1];
	},
	getChapterPageIndex: function (pageIndex) {
		return this.pages[pageIndex][3];
	},
	next: function () {
		if (this.nowPageIndex >= this.pages.length - 1) { return; }
		this.goto(this.nowPageIndex + 1);
	},
	prev: function () {
		if (this.nowPageIndex <= 0) { return; }
		this.goto(this.nowPageIndex - 1);
	},
	getPageCount: function (index) {
		return parseInt(this.chapters.eq(index).find("code").html() || "0", 10);
	},
	setPagePath: function (nodeIndex) {
		var navHTML = [];


		var _node;
		_node = this.chapters.eq(nodeIndex)
		while (_node.length != 0) {
			var _s = _node.get(0).childNodes[0].textContent;
			
			navHTML.push(_s + '');

			if (_node.parent("li").parent("ul").hasClass("ui-fancytree-source")) {
				break;
			} else {
				_node = _node.parent("li").parent("ul").parent("li").find("a").eq(0);
			}
		}
		var pagecount = this.getPageCount(this.nowChapterIndex);


		if (pagecount == 0) {
			this.setPageProgress(-1, -1);
		} else {
			this.setPageProgress(pagecount, this.nowChapterPageIndex)
		}
		$("#tmPagePath").html(navHTML.reverse().join('<span class="icon icon-right"></span>'));
	},
	loadData: function (tmf, id, pageIndex, isShow) {
		tmf.loadingShow();
		$.post("/Teach/getPage/" + id, function (data) {
			var json = eval("("+data+")")
			tmf.setPageContent(pageIndex, json.data);
			if (isShow == true) {
				tmf.updateIndex(pageIndex, tmf.getChapterIndex(pageIndex), tmf.getChapterPageIndex(pageIndex));
				tmf.showPage(pageIndex)
				tmf.pageImgLoadListener();
			}
		})
	},
	loadingShow: function () {
		//windbell.loading.show();
	},
	loadingHide: function () {
		//windbell.loading.hide();
		TMFrame.resetNav();
	},
	pageImgLoadListener: function () {
		var firstImg = $("#tmPageContent img");
		if (firstImg.length > 0) {
			firstImg.eq(0).bind("load", this.loadingHide)
		} else {
			this.loadingHide();
			this.updatePageNavLocal();
		}

	},
	pageImgLoadListenerRemove: function () {
		var firstImg = $("#tmPageContent img");
		if (firstImg.length > 0) {
			firstImg.eq(0).unbind("load", this.loadingHide);
		}
		
		this.loadingHide();
	},
	updateIndex: function (pageIndex, chapterIndex, chapterPageIndex) {
		this.nowPageIndex = pageIndex;
		this.nowChapterIndex = chapterIndex;
		this.nowChapterPageIndex = chapterPageIndex;

		//$("#tree").fancytree("getTree").getNodeByKey("_" + (chapterIndex + 1)).setActive()
	},
	//无page时，显示chapter标题
	showChapterNoPages: function (pageIndex) {
		var chapterIndex = this.getChapterIndex(pageIndex);
		this.setPagePath(chapterIndex);
		$("#tmPageTitle").html(this.chapters.eq(this.getChapterIndex(pageIndex)).text()).show();
		this.updatePageNavLocal();
		//this.resetNav();
	},
	showChapter: function (chapterIndex) {
		for (var i = 0; i < this.pages.length; i++) {
			if (this.getChapterIndex(i) == chapterIndex) {
				this.goto(i);
				return;
			}
		}
	},
	//更新pageNav定位
	updatePageNavLocal: function () {
		//效果不理想 去掉
		//this.pageNav_Headroom.offset = this.pageHeaderBG_height + 10 + $("#tmPageContent").height() - 20;
	},
	//通过cacheIndex，显示page内容
	showPage: function (index) {

		var chapterIndex = this.getChapterIndex(index);
		this.setPagePath(chapterIndex);
		$("#tmPageTitle").hide();
		if (index == undefined) index = this.nowCacheIndex;
		var $node = this.chapters.eq(chapterIndex);


		$("#tmPageTitle").html($node.text())
		$("#tmPageContent").html(this.getPageContent(index)
			//.replace(/src=\"/g, 'src="' + dataUrl)
			);


	},

	setPageProgress: function (pageCount, chapterPageIndex) {
		if (chapterPageIndex >= 0) { chapterPageIndex++; } else { pageCount = 0; chapterPageIndex = 0; }
		var itemWidth = 6;
		var itemHeight = 8;
		$("#tmProgress").width(itemWidth * pageCount).attr("title", pageCount + " - " + chapterPageIndex);
		$("#tmProgress .value").width(itemWidth * chapterPageIndex);
	},

	//清除内容
	reset:function(){
		$("#tmPageTitle").html("");
		$("#tmPageContent").html("");
	},
	//重置上下页
	resetNav: function () {
		//var w = $("#tmPageContent").width();
		//var h = $("#tmPageContent").height() + $("#tmPageTitle").height();
		//$(".tm-pageNav").css({ width: w, height: h });
		//$(".tm-pageNav").css({height: h });
	},
	//放大显示内容IMG
	fullView: function (evt) {
		var el = evt.target;
		if (el.tagName == "IMG") {
			if (window.innerWidth > 1000) { return; }
			if (TMFrame.isFullView) {
				TMFrame.isFullView = false;
				$(".tmPage").addClass("fullView").scrollLeft(0).scrollTop(0);
				//$("#tmPageContent").attr("draggable", true).find("img").css({ width: "100%" });

				//$(".tmPage").removeClass("hiddeScroll").css("overflow", "hidden");
				//$("#tmPageContent").removeClass("scaleView")
			} else {
				TMFrame.isFullView = true;
				$(".tmPage").removeClass("fullView");
				//$("#tmPageContent").addClass("scaleView");
			}
		}
	}
}
var Test = {
	lastWarningTime: 1,//最后警告时间:总时间百分比1=100%;
	usingTime:0,//停留时间
	timer:function(intDiff) { //时间Timer
		window.setInterval(function () {
			var hour = 0,
				minute = 0,
				second = 0;//时间默认值
			if (intDiff > 0) {
				hour = Math.floor(intDiff / (60 * 60));
				minute = Math.floor(intDiff / 60) - (hour * 60);
				second = Math.floor(intDiff) - -(hour * 60 * 60) - (minute * 60);
			}
			if (minute <= 9) minute = '0' + minute;
			if (second <= 9) second = '0' + second;
			$('#hour_show').html(hour);
			$('#minute_show').html(minute);
			$('#second_show').html(second);
			intDiff--;

			if (intDiff < 600 * Test.lastWarning) {
				$(".exam-time").addClass("warning");
			}
			Test.usingTime = parseInt(600 - intDiff);
			if (intDiff == 1) {
				$("#submit").click();
				intDiff = 0;
			}
		}, 1000);
	},
	//答题
	doingQuestion: function (evt) {

		if (evt.target.tagName != "INPUT") { return; }
		var $it = $(evt.target);
		var qa = $it.closest(".q-a");
		if (qa.length !== 0) {
			var item = $it.parents(".q-item");
			var $its = item.find("input");
			var t = $it.attr("type").toLowerCase();
			if (t == "radio") {
				item.addClass("selected");
			} else if (t == "checkbox") {

				if (item.find(":checked").length == 0) {
					item.removeClass("selected")
				} else {
					item.addClass("selected");
				}
			}
			var vals=[]
			$its.each(function (i,v) {
				if ($(this).prop("checked")) {
					vals.push(i);
				}
				
			})
			var valss = vals.join(",");
			if (valss == "" || valss == null) {
				item.attr("an","");
			} else {
				item.attr("an", valss);
			}
		}
	},
	submit:function (testId) { //提交
		var ans = [];
		var lostTop = 0;
		$(".q-item").each(function () {
			var an = $(this).attr("an");
			if (lostTop==0 && an == "") {
				lostTop = $(".content")[0].scrollTop+  $(this).offset().top -50;
			}
			ans.push(an)
		})

		if (lostTop != 0) {
			$.confirm('题未答完，[确认]将强制提交，[取消]继续答题', '确认提交吗？', function () {
				s()
			}, function () {
				$(".content")[0].scrollTop = lostTop
				isSubmit = false;
			});
		} else {
			$.confirm('确认提交吗？', function () {
				s()
			});
		}
		function s(){
			myAjaxPost("/test/testSubmit", { id: testId, answers: ans.join("|") }, function (data) {
				var result = data;
				if (data != null) {
					window.location.href = "/Test/TestResult/"+data;
				}
			})
		}

	}
}
