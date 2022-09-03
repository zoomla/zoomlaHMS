<template>
    <div class="page">
        <div class="mb-3">
            <label class="form_label">主题项目路径</label>
            <div class="input-group">
                <input v-model="config.themeFolder" class="form-control" />
                <button class="btn btn-outline-secondary" v-on:click="openThemeFolder" v-bind:disabled="syncing"><i class="zi zi_floderOpen"></i></button>
                <button class="btn btn-outline-secondary" v-on:click="openFile(config.themeFolder)"><i class="zi zi_externalLinkalt"></i></button>
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">打包的.hwt文件</label>
            <div class="input-group">
                <input v-model="config.themePackage" class="form-control" />
                <button class="btn btn-outline-secondary" v-on:click="openThemePackage" v-bind:disabled="syncing"><i class="zi zi_floderOpen"></i></button>
                <button class="btn btn-outline-secondary" v-on:click="openFile(config.themePackage)"><i class="zi zi_externalLinkalt"></i></button>
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">自动更新文件</label>
            <div class="input-group mb-2">
                <div class="input-group-text">
                    <label class="form-check form-check-inline m-0">
                        <input name="autoRun" type="radio" class="form-check-input" value="inSecond" v-model="config.autoRun" v-bind:disabled="syncing" />
                    </label>
                </div>
                <span class="input-group-text">定时</span>
                <input type="number" min="0" step="1" class="form-control" v-model="config.inSecondSleep" v-bind:disabled="syncing" />
                <span class="input-group-text">秒更新一次</span>
            </div>
            <div class="input-group">
                <div class="input-group-text">
                    <label class="form-check form-check-inline m-0">
                        <input name="autoRun" type="radio" class="form-check-input" value="watchFile" v-model="config.autoRun" v-bind:disabled="syncing" />
                    </label>
                </div>
                <span class="input-group-text">有变化即更新，更新间歇</span>
                <input type="number" min="0" step="1" class="form-control" v-model="config.watchFileSleep" v-bind:disabled="syncing" />
                <span class="input-group-text">秒</span>
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">选择更新文件</label>
            <div>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="icons" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">自定义图标替换</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="unlock-dynamic" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="unlockChoose('unlock-dynamic')" />
                    <span class="form-check-label">动态锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="unlock-magazine" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="unlockChoose('unlock-magazine')" />
                    <span class="form-check-label">杂志锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="unlock-slide" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="unlockChoose('unlock-slide')" />
                    <span class="form-check-label">滑动锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="unlock-video" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="unlockChoose('unlock-video')" />
                    <span class="form-check-label">视频锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="unlock-vr" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="unlockChoose('unlock-vr')" />
                    <span class="form-check-label">VR锁屏</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="wallpaper" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="wallpaperChoose('wallpaper')" />
                    <span class="form-check-label">手机桌面</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="wallpaper-tablet" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="wallpaperChoose('wallpaper-tablet')" />
                    <span class="form-check-label">平板桌面</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="wallpaper-foldable" v-model="config.packFiles" v-bind:disabled="syncing" v-on:click="wallpaperChoose('wallpaper-foldable')" />
                    <span class="form-check-label">折叠屏桌面</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="launcher" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">桌面模块样式</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="contacts" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">联系人及拨号界面样式</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="incallui" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">通话界面样式</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="mms" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">短信模块样式</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="systemui" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">通知模块样式</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="phone" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">com.android.phone</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="telecom" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">com.android.server.telecom</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="recorder" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">com.huawei.phone.recorder</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="famanager" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">com.huawei.ohos.famanager</span>
                </label>
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="widget" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">万象小组件</span>
                </label>
            </div>
        </div>
        <div class="d-flex justify-content-center">
            <div class="btn-group">
                <button class="btn btn-danger" v-on:click="syncEnd" v-if="syncing"><i class="zi zi_stopcircle"></i>停止同步</button>
                <button class="btn btn-success" v-on:click="syncStart" v-else><i class="zi zi_play"></i>开始同步</button>
                <button class="btn btn-info" v-on:click="syncManual" v-bind:disabled="syncing"><i class="zi zi_syncalt"></i>手动同步</button>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                config: {
                    themeFolder: "",
                    themeType: "phone",
                    autoRun: "inSecond",
                    inSecondSleep: 30,
                    watchFileSleep: 30,
                    packFiles: [],
                },
                syncing: false,
            };
        },
        mounted() {
            const that = this;
            cscSetup("ThemeAutoPack");

            csc("GetConfig").then(res => {
                try {
                    let data = JSON.parse(res);
                    const config = that.config;
                    for (let key in data) {
                        config[key] = data[key];
                    }
                } catch (e) {}
            });

            csc("IsSyncing").then(res => {
                that.syncing = res == "1";
            });
        },
        watch: {
            "config": {
                deep: true,
                handler: function(n, o) {
                    csc("SetConfig", JSON.stringify(this.config));
                }
            }
        },
        methods: {
            wallpaperChoose(item) {
                let idx = -1;
                do {
                    idx = this.config.packFiles.findIndex(curr => curr.startsWith("wallpaper") && curr != item);
                    if (idx > -1) {
                        this.config.packFiles.splice(idx, 1);
                    }
                } while(idx > -1);

                idx = this.config.packFiles.findIndex(curr => curr == item);
                if (idx > -1) {
                    this.config.packFiles.splice(idx, 1);
                } else {
                    this.config.packFiles.push(item);
                }
            },
            unlockChoose(item) {
                let idx = -1;
                do {
                    idx = this.config.packFiles.findIndex(curr => curr.startsWith("unlock") && curr != item);
                    if (idx > -1) {
                        this.config.packFiles.splice(idx, 1);
                    }
                } while(idx > -1);

                idx = this.config.packFiles.findIndex(curr => curr == item);
                if (idx > -1) {
                    this.config.packFiles.splice(idx, 1);
                } else {
                    this.config.packFiles.push(item);
                }
            },
            openThemeFolder() {
                const that = this;
                csc("ChooseThemeFolder").then(res => {
                    that.config.themeFolder = res;
                });
            },
            openThemePackage() {
                const that = this;
                csc("ChooseThemePackage").then(res => {
                    that.config.themePackage = res;
                });
            },
            syncStart() {
                const that = this;
                csc("StartSync").then(res => {
                    if (res == "1") {
                        that.syncing = true;
                    }
                });
            },
            syncEnd() {
                const that = this;
                csc("StopSync").then(res => {
                    if (res == "1") {
                        that.syncing = false;
                    }
                });
            },
            syncManual() {
                app.showLoading();
                csc("ManualSync").then(res => {
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
