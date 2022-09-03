<template>
    <div class="page">
        <div class="links">
            <a href="javascript:;" v-for="item in links" v-bind:key="item"><i class="zi zi_hashtag"></i> {{item.label}}</a>
        </div>
        <div class="append">
            <a class="link-secondary" href="javascript:;">更多资源 <i class="zi zi_handRight"></i></a>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                links: [
                    {label: "简单静态主题", href: ""},
                    {label: "AOD主题示例", href: ""},
                    {label: "基础动态锁屏", href: ""},
                    {label: "动态锁屏+多屏切换", href: ""},
                    {label: "动态锁屏+充电动效", href: ""},
                    {label: "动态锁屏+时分倒计时", href: ""},
                    {label: "动态锁屏+点按唤起应用", href: ""},
                    {label: "动态锁屏+桌面壁纸联动", href: ""},
                    {label: "基础动态桌面", href: ""},
                    {label: "动态桌面+水波纹特效", href: ""},
                ]
            };
        },
        mounted() {
        },
        methods: {
            opeUrl() {},
        },
    }
</script>

<style>
.page{background:#f8f9fa; height:100%;}

.links{display:flex; flex-wrap:wrap; margin-bottom:2rem;}
.links>a{display:block; width:140px; min-height:90px; padding:1rem; margin:0 .5rem .5rem 0; color:#343a40; background:#fff; border-radius:.375rem; text-decoration:none; cursor:pointer;}
.links>a:hover{background:#e9ecef;}
.links>a>.zi{color:#000;}

.append{text-align:center;}
</style>
