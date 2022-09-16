<template>
    <div class="page">
        <div class="mb-3">
            <label class="form_label">主题项目位置</label>
            <div class="input-group">
                <input name="projectPath" v-model="config.themeFolder" class="form-control" />
                <button class="btn btn-outline-secondary" v-on:click="openThemeFolder"><i class="zi zi_floderOpen"></i></button>
                <button class="btn btn-outline-secondary" v-on:click="openFile(config.themeFolder)"><i class="zi zi_eye"></i></button>
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">打包输出位置</label>
            <div class="input-group">
                <input name="exportPath" v-model="config.themeExportFolder" class="form-control" />
                <button class="btn btn-outline-secondary" v-on:click="openThemeExportFolder"><i class="zi zi_floderOpen"></i></button>
                <button class="btn btn-outline-secondary" v-on:click="openFile(config.themeExportFolder)"><i class="zi zi_eye"></i></button>
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">适用设备类型</label>
            <div>
                <label class="form-check form-check-inline">
                    <input name="device" type="checkbox" class="form-check-input" value="phone" v-model="config.applyDevices" />
                    <span class="form-check-label">手机</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="device" type="checkbox" class="form-check-input" value="tablet" v-model="config.applyDevices" />
                    <span class="form-check-label">平板</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="device" type="checkbox" class="form-check-input" value="foldable" v-model="config.applyDevices" />
                    <span class="form-check-label">折叠屏</span>
                </label>
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">对应系统版本</label>
            <div class="d-flex">
                <!-- <label class="form-check form-check-inline">
                    <input name="os" type="radio" class="form-check-input" value="10.0.100" v-model="config.applySystem" />
                    <span class="form-check-label">EMUI10.1</span>
                </label> -->
                <label class="form-check form-check-inline">
                    <input name="os" type="radio" class="form-check-input" value="11.0" v-model="config.applySystem" />
                    <span class="form-check-label">EMUI11.0</span>
                </label>
                <!-- <label class="form-check form-check-inline">
                    <input name="os" type="radio" class="form-check-input" value="12.0" v-model="config.applySystem" />
                    <span class="form-check-label">Harmony2.0</span>
                </label> -->
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">主题项目类型</label>
            <div class="d-flex">
                <label class="form-check form-check-inline">
                    <input name="type" type="radio" class="form-check-input" value="largeScope" v-model="config.themeType" />
                    <span class="form-check-label">大主题</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="type" type="radio" class="form-check-input" value="smallScope" v-model="config.themeType" />
                    <span class="form-check-label">小主题</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="type" type="radio" class="form-check-input" value="icon" v-model="config.themeType" />
                    <span class="form-check-label">图标</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="type" type="radio" class="form-check-input" value="lockScreen" v-model="config.themeType" />
                    <span class="form-check-label">锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="type" type="radio" class="form-check-input" value="aod" v-model="config.themeType" disabled />
                    <span class="form-check-label">AOD</span>
                </label>
            </div>
        </div>
        <div class="mb-3" v-if="config.themeType == 'largeScope' || config.themeType == 'smallScope' || config.themeType == 'lockScreen'">
            <label class="form_label">主题锁屏类型</label>
            <div class="d-flex">
                <label class="form-check form-check-inline">
                    <input name="lockscreen" type="radio" class="form-check-input" value="dynamic" v-model="config.themeLockscreen" />
                    <span class="form-check-label">动态锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="lockscreen" type="radio" class="form-check-input" value="magazine" v-model="config.themeLockscreen" />
                    <span class="form-check-label">杂志锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="lockscreen" type="radio" class="form-check-input" value="slide" v-model="config.themeLockscreen" />
                    <span class="form-check-label">滑动锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="lockscreen" type="radio" class="form-check-input" value="video" v-model="config.themeLockscreen" />
                    <span class="form-check-label">视频锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="lockscreen" type="radio" class="form-check-input" value="vr" v-model="config.themeLockscreen" />
                    <span class="form-check-label">VR锁屏</span>
                </label>
            </div>
        </div>
        <div class="text-center">
            <button class="btn btn-info" v-on:click="packStart"><i class="zi zi_boxopen"></i>开始打包</button>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                config: {
                    themeFolder: "",
                    themeExportFolder: "",
                    applyDevices: [],
                    applySystem: "",
                    themeType: "",
                    themeLockscreen: "",
                }
            };
        },
        mounted() {
            const that = this;
            CSharp.call("ThemePack.GetConfig").then(res => {
                res = JSON.parse(res);
                const config = that.config;
                for (let key in res) {
                    config[key] = res[key] || config[key];
                }
            });
        },
        watch: {
            "config": {
                deep: true,
                handler: function(o, n) {
                    CSharp.call("ThemePack.SetConfig", JSON.stringify(this.config));
                }
            }
        },
        methods: {
            openThemeFolder() {
                const that = this;
                CSharp.call("ThemePack.ChooseThemeFolder").then(res => {
                    that.config.themeFolder = res;
                });
            },
            openThemeExportFolder() {
                const that = this;
                CSharp.call("ThemePack.ChooseThemeExportFolder").then(res => {
                    that.config.themeExportFolder = res;
                });
            },
            packStart() {
                app.showLoading();
                CSharp.call("ThemePack.PackTheme").then(res => {
                    app.hideLoading();
                });
            },
            openFile(path) {
                CSharp.openFile(path);
            },
        },
    }
</script>

<style>
</style>
