<template>
    <div class="page">
        <!-- <div v-on:click="jump('extend/hwtviewer')">主题包审计</div>
        <div v-on:click="jump('themepack/syncmobile')">推送主题包到手机</div>
        <div v-on:click="jump('extend/takecolor')">屏幕取色</div> -->
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
            }
        },
    }
</script>

<style>
.page{display:flex; flex-wrap:wrap; align-items:flex-start;}
.page>div{padding:1rem; margin:0 .5rem .5rem 0; color:#495057; background:#f8f9fa; border-radius:.375rem; cursor:pointer; user-select:none;}
.page>div:hover{background:#e9ecef;}
</style>
