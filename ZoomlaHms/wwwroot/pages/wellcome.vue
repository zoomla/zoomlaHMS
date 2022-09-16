<template>
    <div class="page">
        <div class="well_content">
            <div class="well_title">欢迎使用逐浪HMS主题大师</div>
            <div class="well_quick_funcs">
                <div class="well_quick_funcs_item" v-on:click="open('extend/online', '实用资源')">
                    <i class="zi zi_paintbrush"></i>
                    <span>实用资源</span>
                </div>
                <div class="well_quick_funcs_item" v-on:click="open('themepack/paksync', '主题包同步')">
                    <i class="zi zi_boxopen"></i>
                    <span>主题包同步</span>
                </div>
            </div>
            <div class="well_quick_funcs">
                <div class="well_quick_funcs_item" v-on:click="open('themepack/syncmobile', '手机推送工具')">
                    <i class="zi zi_exchangealt"></i>
                    <span>手机推送工具</span>
                </div>
                <div class="well_quick_funcs_item" v-on:click="jump('help')">
                    <i class="zi zi_circleQuestion"></i>
                    <span>使用帮助</span>
                </div>
            </div>
            <div class="well_say" v-if="says">“{{says}}”</div>
        </div>
        <div class="cright">
            <span>基于</span>
            <a href="javascript:;" class="link-secondary" v-on:click="openUrl('https://ziti163.com/')">逐浪字库</a>
            <span>与</span>
            <a href="javascript:;" class="link-secondary" v-on:click="openUrl('https://ico.z01.com/')">zico图标</a>
            <span>呈现∞</span>
            <a href="javascript:;" class="link-secondary" v-on:click="openUrl('https://z01.com/')">Zoomla!逐浪CMS</a>
            <span>官方技术支持</span>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                says: "",
            };
        },
        mounted() {
            const that = this;
            cscSetup("Helpful");

            this.getSay();
            window.addEventListener("wellcome", this.getSay);
        },
        unmounted() {
            window.removeEventListener("wellcome", this.getSay);
        },
        methods: {
            jump(url) {
                app.navigateTo(url);
            },
            open(url, title) {
                CSharp.singleModuleView(url, title);
            },
            noop() {
                CSharp.prompt("暂未开放");
            },
            getSay() {
                const that = this;

                csc("GetSay").then(res => {
                    that.says = res;
                });
            },
            openUrl(url) {
                CSharp.openUrl(url);
            },
        },
    }
</script>

<style>
.page{display:flex; flex-flow:column; justify-content:center; align-items:center; height:100vh; padding:1rem;}
.well_content{width:100%; max-width:600px; min-height:76vh;}
.well_title{padding:1.5rem; font-size:1.5rem; text-align:center;}
.well_quick_funcs{display:flex; justify-content:center;}
.well_quick_funcs_item{display:flex; flex-flow:column; justify-content:center; align-items:center; width:180px; height:100px; margin:0 .5rem .5rem 0; font-size:.9rem; color:#6c757d; background:#F7F8F9; border-radius:.375rem; cursor:pointer;}
.well_quick_funcs_item>.zi{font-size:2rem;}
.well_quick_funcs_item:last-child{margin-right:0;}
.well_quick_funcs_item:hover{color:#495057; background:#EEF1F3;}
.well_say{margin-top:3rem; text-align:center; color:#6c757d; font-style:italic;}

.cright{font-size:.85rem; color:#91989E;}
.cright>a{margin:0 .1rem; color:inherit !important;}
</style>
