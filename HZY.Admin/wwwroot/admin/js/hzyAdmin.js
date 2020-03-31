/*
 * *******************************************************
 *
 * 作者：hzy
 *
 * 开源地址：https://gitee.com/hzy6
 *
 * *******************************************************
 */
var hzyAdmin = {
    layer: null,
    vuePro: null,
    init: function () {
        this.layer = top.layer;
        this.vuePro = top.Vue.prototype;
        var _this = this;
        //ajax 全局 拦截
        $.ajaxSetup({
            dataType: "json",
            cache: false,
            xhrFields: {
                withCredentials: true
            },
            complete: function (xhr) {
                var resJson = xhr.responseJSON;
                console.log('拦截器', resJson);
                if (resJson.hasOwnProperty('status')) {
                    if (resJson.status == -1) { //接口授权码无效
                        if (confirm(resJson.msg + ',请重新登录授权!'))
                            window.location = "/Authorization/Out";
                        return;
                    }
                    if (resJson.status == -2) { //服务端异常
                        _this.alert(resJson.msg, '错误');
                        return;
                    }
                    if (resJson.status == 0) { //失败
                        _this.alert(resJson.msg, '警告');
                        return;
                    }
                }
            },
            error: function (event, xhr, options, exc) {
                //event - 包含 event 对象
                //xhr - 包含 XMLHttpRequest 对象
                //options - 包含 AJAX 请求中使用的选项
                //exc - 包含 JavaScript exception
                if (xhr.status != 200) {
                    console.log(event, xhr, options, exc);
                    _this.loading.stop();
                }
            }
        });
    },
    //加载标签页面
    toView: function (id, title, href, active = true, isClose = true) {
        top.hzyBootStrapTabs.addTab(id, title, href, true, isClose);
    },
    loading: {
        index: null,
        start: function (id = 'hzyadmin-loading') {
            this.index = hzyAdmin.vuePro.$loading({
                fullscreen: true,
                background: 'rgba(255,255,255,.1)'
            });
        },
        stop: function (id = 'hzyadmin-loading') {
            if (this.index) this.index.close();
        }
    },
    alert: function (text, type = 'info', title = '消息提示') {
        console.log(type);
        var _this = this;
        if (type == '成功') {
            _this.vuePro.$message({
                message: text,
                type: 'success',
                duration: 5000
            });
            return;
        }
        if (type == '错误') {
            _this.vuePro.$message({
                message: text,
                type: 'error',
                duration: 5000
            });
            return;
        }
        if (type == '警告') {
            _this.vuePro.$message({
                message: text,
                type: 'warning',
                duration: 5000
            });
            return;
        }
        _this.vuePro.$message({
            message: text,
            duration: 5000
        });
    },
    post: function (url, data, success, loading = true) {
        var _this = this;
        if (loading) top.hzyAdmin.loading.start();
        $.ajax({
            type: "post",
            dataType: "json",
            contentType: 'application/json;charset=UTF-8',
            url: url,
            data: JSON.stringify(data),
            success: function (r) {
                if (loading) top.hzyAdmin.loading.stop();
                if (success) success(r);
            }
        });
    },
    upload: function (url, data, success, loading = true) {
        var _this = this;
        if (loading) _this.loading.start();
        $.ajax({
            type: "post",
            dataType: "json",
            contentType: 'multipart/form-data',
            url: url,
            data: data,
            processData: false,
            contentType: false,
            cache: false,
            success: function (r) {
                if (loading) _this.loading.stop();
                if (success) success(r);
            }
        });
    },
    download: function (url, data, loading = true) {
        var _this = this;
        if (loading) _this.loading.start();
        $.ajax({
            type: "get",
            contentType: 'application/json; charset=UTF-8',
            xhrFields: {
                responseType: 'blob'
            },
            url: url,
            data: JSON.stringify(data),
            success: function (data) {
                if (loading) _this.loading.stop();
                var headers = res.headers;
                //"attachment; filename=6a9c13bc-e214-44e4-8456-dbca9fcd2367.xls;filename*=UTF-8''6a9c13bc-e214-44e4-8456-dbca9fcd2367.xls"
                var contentDisposition = headers['content-disposition'];
                var contentType = headers['content-type'];
                var attachmentInfoArrary = contentDisposition.split(';');
                var fileName = '';
                if (attachmentInfoArrary.length > 1) {
                    fileName = attachmentInfoArrary[1].split('=')[1];
                }
                var blob = new Blob([data], { type: contentType });

                if (window.navigator && window.navigator.msSaveOrOpenBlob) { // IE
                    window.navigator.msSaveOrOpenBlob(blob, fileName);
                } else {
                    var url = (window.URL || window.webkitURL).createObjectURL(blob);
                    // window.open(url, "_blank"); //下载
                    // window.URL.revokeObjectURL(url) // 只要映射存在，Blob就不能进行垃圾回收，因此一旦不再需要引用，就必须小心撤销URL，释放掉blob对象。

                    var a = document.createElement('a');
                    a.style.display = 'none';
                    a.href = url;
                    a.setAttribute('download', fileName);
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a); // 下载完成移除元素
                    // window.location.href = url
                    window.URL.revokeObjectURL(url); // 只要映射存在，Blob就不能进行垃圾回收，因此一旦不再需要引用，就必须小心撤销URL，释放掉blob对象。
                }
            }
        });
    },
    //根据键取地址栏中的值
    getQueryString: function (key) {
        var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return r[2]; return "";
    },
    //建立一個可存取到該file的url  用于上传图片，，可通过该地址浏览图片
    getObjectUrl: function (file) {
        var url = "";
        if (window.createObjectURL != undefined) { // basic
            url = window.createObjectURL(file);
        } else if (window.URL != undefined) { // mozilla(firefox)
            url = window.URL.createObjectURL(file);
        } else if (window.webkitURL != undefined) { // webkit or chrome
            url = window.webkitURL.createObjectURL(file);
        }
        return url;
    },
    //将图片对象转换为 base64
    readFile: function (obj, callBack) {
        var file = obj.files[0];
        var resVal;
        //判断类型是不是图片  
        if (!/image\/\w+/.test(file.type)) {
            alert("请确保文件为图像类型");
            return false;
        }
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            //alert(this.result); //就是base64  
            resVal = this.result;
            if (callBack) callBack(resVal);
            //return resVal;
        }

    },
    setSession: function (key, value) {
        window.top.localStorage.setItem(key, JSON.stringify(value));
    },
    getSession: function (key) {
        return JSON.parse(window.top.localStorage.getItem(key));
    },
    getCookie: function (name) {
        var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
        if (arr = top.document.cookie.match(reg))
            return unescape(arr[2]);
        else
            return null;
    },
    delCookie: function (name) {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = getCookie(name);
        if (cval != null)
            top.document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
    },
    //这是有设定过期时间的使用示例：
    //s20是代表20秒
    //h是指小时，如12小时则是：h12
    //d是天数，30天则：d30
    setCookie: function (name, value, time, path) {
        if (!time) time = 'h12';
        var strsec = hzyAdmin.getsec(time);
        var exp = new Date();
        exp.setTime(exp.getTime() + strsec * 1);
        top.document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString() + (path ? (";path=" + path) : ";path=/");

    },
    getsec: function (str) {
        var str1 = str.substring(1, str.length) * 1;
        var str2 = str.substring(0, 1);
        if (str2 == "s") {
            return str1 * 1000;
        }
        else if (str2 == "h") {
            return str1 * 60 * 60 * 1000;
        }
        else if (str2 == "d") {
            return str1 * 24 * 60 * 60 * 1000;
        }
    },
    openPage: function (title, href, end = null, w = 500, h = 600) {
        var _htmlW = top.$("html").width();
        var _htmlH = top.$("html").height();
        var _width = _htmlW > w ? w : _htmlW;
        var _height = _htmlH > w ? h : _htmlH;
        hzyAdmin.layer.open({
            type: 2,
            title: title,
            area: [_width + 'px', _height + 'px'],
            fixed: false, //不固定
            maxmin: true,
            content: href,
            end: end
        });
    }

};

Vue.prototype.$ELEMENT = { size: 'small', zIndex: 99999999 };

hzyAdmin.init();
