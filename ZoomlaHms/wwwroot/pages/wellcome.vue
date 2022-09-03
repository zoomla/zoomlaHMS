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
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {};
        },
        mounted() {
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
        },
    }
</script>

<style>
.page{display:flex; justify-content:center; align-items:center; height:100vh; padding:1rem;}
.well_content{width:100%; max-width:600px; min-height:76vh;}
.well_title{padding:1.5rem; font-size:1.5rem; text-align:center;}
.well_quick_funcs{display:flex; justify-content:center;}
.well_quick_funcs_item{display:flex; flex-flow:column; justify-content:center; align-items:center; width:180px; height:100px; margin:0 .5rem .5rem 0; font-size:.9rem; color:#6c757d; background:#F7F8F9; border-radius:.375rem; cursor:pointer;}
.well_quick_funcs_item>.zi{font-size:2rem;}
.well_quick_funcs_item:last-child{margin-right:0;}
.well_quick_funcs_item:hover{color:#495057; background:#EEF1F3;}
</style>
