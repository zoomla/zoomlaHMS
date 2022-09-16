<template>
    <div class="page">
        <div class="title">
            <label>ZoomLa HMS</label>
            <span v-on:click="test">HMS主题开发实用工具集</span>
        </div>

        <div class="content">
            <!-- 快捷键 -->
            <div class="content_item">
                <label>程序快捷键</label>
                <div class="hotkey">
                    <div class="hotkey_item" v-for="item in hotkey" v-bind:key="item">
                        <span class="code" v-if="item.ctrl">ctrl</span>
                        <i class="ico" v-if="item.ctrl"></i>
                        <span class="code" v-if="item.alt">alt</span>
                        <i class="ico" v-if="item.alt"></i>
                        <span class="code" v-if="item.shift">shift</span>
                        <i class="ico" v-if="item.shift"></i>
                        <span class="code" v-if="item.win">win</span>
                        <i class="ico" v-if="item.win"></i>
                        <span class="code" v-if="item.other">{{item.other}}</span>
                        <span>：{{item.name}}</span>
                    </div>
                </div>
            </div>
            <!-- 按钮 -->
            <div class="content_item">
                <label>其他</label>
                <div class="hbtns">
                    <a href="javascript:;" v-on:click="openUrl('https://developer.huawei.com/consumer/cn/doc/distribution/content/themes-engine-overview-0000001054588463')">主题开发文档</a>
                    <a href="javascript:;" v-on:click="openUrl('https://developer.huawei.com/consumer/cn/doc/Tools-Library/theme_download-0000001050424897')">主题开发工具</a>
                    <a href="javascript:;" v-on:click="reload">重新加载框架</a>
                    <a href="javascript:;" v-on:click="seelog">查看程序日志</a>
                    <a href="javascript:;" v-on:click="policy">软件许可协议</a>
                </div>
            </div>
        </div>

        <div class="copyright">
            <div>
                <span>Powered by</span>
                <a href="javascript:;" class="link-secondary" v-on:click="openUrl('https://dotnet.microsoft.com/')">.NET 7</a>
                <span>& CefSharp</span>
            </div>
            <div>
                <span>基于</span>
                <a href="javascript:;" class="link-secondary" v-on:click="openUrl('https://ziti163.com/')">逐浪字库</a>
                <span>与</span>
                <a href="javascript:;" class="link-secondary" v-on:click="openUrl('https://ico.z01.com/')">zico图标</a>
                <span>呈现∞</span>
                <a href="javascript:;" class="link-secondary" v-on:click="openUrl('https://z01.com/')">Zoomla!逐浪CMS</a>
                <span>官方技术支持</span>
            </div>
            <div>
                <span>电话：13177777714</span>
                <span>QQ：1799661890</span>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                hotkey: [
                    // {ctrl: true, alt: true, shift: false, win: false, other: "", name: ""},
                    {ctrl: true, alt: true, shift: false, win: false, other: "数字1", name: "显示主窗口"},
                    {ctrl: true, alt: true, shift: false, win: false, other: "数字2", name: "主题包同步"},
                    {ctrl: true, alt: true, shift: false, win: false, other: "数字3", name: "手机推送工具"},
                    {ctrl: true, alt: true, shift: false, win: false, other: "数字4", name: "主题包审计"},
                    {ctrl: true, alt: true, shift: false, win: false, other: "数字5", name: "主题打包"},
                    {ctrl: true, alt: true, shift: false, win: false, other: "数字6", name: "屏幕取色"},
                    {ctrl: true, alt: true, shift: false, win: false, other: "数字7", name: "命令行版推送工具"},
                ],
            };
        },
        mounted() {
            cscSetup("Helpful");
        },
        methods: {
            openUrl(url) {
                CSharp.openUrl(url);
            },
            reload() {
                location.reload();
            },
            seelog() {
                csc("OpenLogView");
            },
            policy() {
                csc("ShowPolicy");
            },
            test() {
                CSharp.sayHello();
            },
        },
    }
</script>

<style>
.title{margin-bottom:1rem; font-size:1.5rem;}
.title>span{margin-left:1rem; font-size:1rem; color:#6c757d;}

.content{width:100%; margin-bottom:1rem;}
.content_item{margin-bottom:1rem;}
.content_item>label{margin-bottom:.5rem; color:#495057; font-style:italic;}
.content_item .ico{display:inline-block; width:18px; height:18px; margin:0 .15rem; background-size:100%; background-image:url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIGNsYXNzPSJiaSBiaS1wbHVzLWxnIiB2aWV3Qm94PSIwIDAgMTYgMTYiPjxwYXRoIGZpbGwtcnVsZT0iZXZlbm9kZCIgZD0iTTggMmEuNS41IDAgMCAxIC41LjV2NWg1YS41LjUgMCAwIDEgMCAxaC01djVhLjUuNSAwIDAgMS0xIDB2LTVoLTVhLjUuNSAwIDAgMSAwLTFoNXYtNUEuNS41IDAgMCAxIDggMloiLz48L3N2Zz4=');}
.content_item .code{padding:0 .375rem; color:#495057; background:#ffe6e6; border-radius:.25rem;}

.hotkey{width:100%;}
.hotkey_item{display:flex; align-items:center; padding-left:1rem; margin-bottom:.375rem; position:relative; vertical-align:middle;}
.hotkey_item::before{content:""; display:block; width:6px; height:6px; position:absolute; top:10px; left:2px; background:#adb5bd; border-radius:50%;}

.hbtns{display:flex; flex-flow:column; align-items:flex-start;}
.hbtns>a{padding:.5rem 1.375rem; margin-bottom:.5rem; color:#6c757d; background:#f3f4f6; text-decoration:none; border-radius:.25rem; cursor:pointer;}
.hbtns>a:last-child{margin-right:0;}

.copyright{margin-bottom:1.5rem; font-size:.9rem; color:#343a40; text-align:center;}
.copyright a{margin:0 .2rem;}
.copyright>.team{font-size:1rem;}

@media (min-width:920px) {
    .content{display:flex; flex-wrap:wrap; width:100%; margin-bottom:3rem;}
    .content_item{width:calc(100% / 2 - 1rem + 1rem / 2); margin-right:1rem;}
    .content_item:nth-child(2n){margin-right:0;}
}
</style>
