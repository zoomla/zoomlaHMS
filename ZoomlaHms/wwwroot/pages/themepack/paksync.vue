<template>
    <div class="page">
        <div class="mb-3">
            <label class="form_label">主题项目路径</label>
            <div class="input-group">
                <input v-model="config.themeFolder" class="form-control" />
                <button class="btn btn-outline-secondary" v-on:click="openThemeFolder" v-bind:disabled="syncing"><i class="zi zi_floderOpen"></i></button>
                <button class="btn btn-outline-secondary" v-on:click="openFile(config.themeFolder)"><i class="zi zi_eye"></i></button>
            </div>
        </div>
        <div class="mb-3">
            <label class="form_label">打包的.hwt文件</label>
            <div class="input-group">
                <input v-model="config.themePackage" class="form-control" />
                <button class="btn btn-outline-secondary" v-on:click="openThemePackage" v-bind:disabled="syncing"><i class="zi zi_floderOpen"></i></button>
                <button class="btn btn-outline-secondary" v-on:click="openFile(config.themePackage)"><i class="zi zi_eye"></i></button>
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
            <label class="form_label">选择更新文件
                <small class="text-primary ms-2 text-decoration-underline" style="cursor:pointer;" v-on:click="checkAll">一键全选</small>
                <small class="text-danger ms-2 text-decoration-underline" style="cursor:pointer;" v-on:click="checkAllCancel">一键反选</small>
                <small class="text-success ms-2 text-decoration-underline" style="cursor:pointer;" v-on:click="checkMode('mobile')">手机主题包</small>
            </label>
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
                <!-- <label class="form-check form-check-inline">
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
                </label> -->
                <label class="form-check form-check-inline">
                    <input name="upfile" type="checkbox" class="form-check-input" value="widget" v-model="config.packFiles" v-bind:disabled="syncing" />
                    <span class="form-check-label">万象小组件</span>
                </label>
            </div>
        </div>
        <div class="d-flex justify-content-center position-relative">
            <div style="position:absolute; top:0; left:calc(50% - 20rem - 10px);" v-if="lastPackTime">
                <div class="text-muted small">最后打包：{{lastPackTime}}</div>
                <div class="d-flex align-items-center">
                    <div class="progress w-100">
                        <div class="progress-bar progress-bar-striped" v-if="packSizeRate <= 60"
                            v-bind:style="{'width':packSizeRate+'%'}"></div>
                        <div class="progress-bar progress-bar-striped bg-warning" v-if="packSizeRate > 60 && packSizeRate <= 85"
                            v-bind:style="{'width':packSizeRate+'%'}"></div>
                        <div class="progress-bar progress-bar-striped bg-danger" v-if="packSizeRate > 85"
                            v-bind:style="{'width':packSizeRate+'%'}"></div>
                    </div>
                    <div class="small text-muted text-end" style="flex-shrink:0; width:6rem;">{{packSize.toFixed(1)}}M/{{packSizeMax}}M</div>
                </div>
            </div>
            <div class="btn-group">
                <button class="btn btn-danger" v-on:click="syncEnd" v-if="syncing"><i class="zi zi_stopcircle"></i>停止同步</button>
                <button class="btn btn-success" v-on:click="syncStart" v-else><i class="zi zi_play"></i>开始同步</button>
                <button class="btn btn-info" v-on:click="syncManual" v-bind:disabled="syncing"><i class="zi zi_syncalt"></i>手动同步</button>
            </div>
            <a href="javascript:;" class="small link-secondary fst-italic" style="position:absolute; right:0; bottom:0;" v-on:click="showLogs('now')">查看同步日志</a>
        </div>

        <div class="modal d-block" v-bind:class="packLog.isreport ? 'modal-xl' : 'modal-lg'" v-if="packLog.show">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content" v-if="!packLog.isreport">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <span>{{packLog.time}} 总计同步{{packLog.list.length}}次</span>
                            <a href="javascript:;" class="ms-3" style="font-size:.85rem;" v-on:click="showLogFiles">历史记录</a>
                        </h5>
                        <a href="javascript:;" class="btn-close" v-on:click="hideLogs"></a>
                    </div>
                    <div class="modal-body p-0" style="min-height:40vh;">
                        <div class="packlog" v-for="item in packLog.list" v-bind:key="item">
                            <div class="packlog_time">{{item.time}}</div>
                            <div class="packlog_txt" v-bind:class="item.code == 0 ? 'error' : 'success'">
                                <span>同步状态：</span>
                                <span>{{item.message}}</span>
                            </div>
                            <div class="packlog_txt">
                                <span>主题路径：</span>
                                <span>{{item.project}}</span>
                            </div>
                            <div class="packlog_txt">
                                <span>打包路径：</span>
                                <span>{{item.package}}</span>
                            </div>
                            <div class="packlog_size" v-if="item.code == 1">
                                <div class="progress w-100">
                                    <div class="progress-bar progress-bar-striped" v-if="item.sizeRate <= 60"
                                        v-bind:style="{'width':item.sizeRate+'%'}"></div>
                                    <div class="progress-bar progress-bar-striped bg-warning" v-if="item.sizeRate > 60 && item.sizeRate <= 85"
                                        v-bind:style="{'width':item.sizeRate+'%'}"></div>
                                    <div class="progress-bar progress-bar-striped bg-danger" v-if="item.sizeRate > 85"
                                        v-bind:style="{'width':item.sizeRate+'%'}"></div>
                                </div>
                                <div class="packlog_size_txt">{{item.size.toFixed(1)}}M/{{packSizeMax}}M</div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-content" v-else>
                    <div class="modal-header">
                        <h5 class="modal-title">近30天打包记录</h5>
                        <a href="javascript:;" class="btn-close" v-on:click="hideLogs"></a>
                    </div>
                    <div class="modal-body" style="min-height:40vh;">
                        <div class="d-flex flex-wrap align-items-start justify-content-start">
                            <div class="packlogfile" v-for="item in packLog.reports" v-bind:key="item" v-on:click="showLogs(item.item1)"
                                v-bind:style="{'background':'rgba(255,193,7, ' + (0.2 + 0.6 * item.rate) + ')'}">
                                <p class="packlogfile_name">{{item.item1}}</p>
                                <span class="packlogfile_lines">打包次数：{{item.item2}}</span>
                            </div>
                        </div>
                    </div>
                </div>
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
                timer: null,
                lastPackTime: "",
                packSize: 0,
                packSizeMax: 90,
                packLog: {
                    show: false,
                    time: "",
                    list: [],

                    isreport: false,
                    reports: [],
                },
            };
        },
        computed: {
            packSizeRate: function() {
                let packSize = this.packSize;
                let packSizeMax = this.packSizeMax;
                return packSize / packSizeMax > 1 ? 100 : packSize / packSizeMax < 0 ? 0 : packSize/ packSizeMax * 100;
            },
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

            this.timer = setInterval(() => {
                csc("GetPackagedTime").then(res => {
                    if (res.startsWith("0")) {
                        that.lastPackTime = "";
                        return;
                    }

                    if (that.lastPackTime != res) {
                        csc("GetPackageSize").then(res => {
                            let size = parseInt(res);
                            if (res == -1) {
                                that.packSize = size;
                                return;
                            }

                            that.packSize = size / 1024 / 1024;
                        });
                    }
                    that.lastPackTime = res;
                });
            }, 500);
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
            checkAll() {
                if (this.syncing) {
                    return;
                }

                const list = this.config.packFiles;
                //"icons", "launcher", "contacts", "incallui", "mms", "systemui", "phone", "telecom", "recorder", "famanager", "widget"
                const oncelist = ["icons", "launcher", "contacts", "incallui", "mms", "systemui", "widget"];
                for (let i = 0; i < oncelist.length; i++) {
                    const item = oncelist[i];
                    if (list.indexOf(item) == -1) {
                        list.push(item);
                    }
                }

                if (list.findIndex(curr => curr.startsWith("unlock")) == -1) {
                    list.push("unlock-dynamic");
                }
                if (list.findIndex(curr => curr.startsWith("wallpaper")) == -1) {
                    list.push("wallpaper");
                }
            },
            checkAllCancel() {
                if (this.syncing) {
                    return;
                }

                const list = this.config.packFiles;
                list.splice(0, list.length);
            },
            checkMode(name) {
                if (this.syncing) {
                    return;
                }
                this.checkAllCancel();

                const list = this.config.packFiles;
                switch (name) {
                    case "mobile":
                        {
                            list.push("icons");
                            list.push("unlock-dynamic");
                            list.push("wallpaper");
                            list.push("widget");
                        }
                        break;
                
                    default:
                        break;
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
            showLogFiles() {
                const that = this;
                this.packLog.show = false;
                app.showLoading();

                csc("GetPackLogFileList").then(res => {
                    const packLog = that.packLog;

                    var now = new Date();
                    now.setMilliseconds(0);
                    now.setSeconds(0);
                    now.setMinutes(0);
                    now.setHours(8);

                    let arr = JSON.parse(res);
                    let nowIdx = arr.findIndex(item => Date.parse(item.item1) == now.getTime());
                    if (nowIdx > -1) {
                        arr[nowIdx].item1 = "今日";
                    } else {
                        arr.push({
                            item1: "今日",
                            item2: 0,
                        });
                    }

                    let max = 0;
                    for (let i = 0; i < arr.length; i++) {
                        const item = arr[i];
                        if (item.item2 > max) {
                            max = item.item2;
                        }
                    }
                    for (let j = 0; j < arr.length; j++) {
                        const item = arr[j];
                        if (max == 0) {
                            item.rate = 100;
                            continue;
                        }

                        item.rate = item.item2 / max;
                    }

                    packLog.reports.splice(0, packLog.reports.length);
                    packLog.reports.push.apply(packLog.reports, arr);

                    packLog.isreport = true;
                    packLog.show = true;
                });
            },
            showLogs(time) {
                const that = this;
                this.packLog.show = false;
                app.showLoading();

                var now = new Date();
                now.setMilliseconds(0);
                now.setSeconds(0);
                now.setMinutes(0);
                now.setHours(8);

                if (time == "now" || time == "今日" || Date.parse(time) == now.getTime()) {
                    this.packLog.time = "今日";
                } else {
                    this.packLog.time = time;
                }
                if (time == "今日") {
                    time = "now";
                }

                csc("GetPackLog", time + "::string").then(res => {
                    const packLog = that.packLog;

                    packLog.list.splice(0, packLog.list.length);
                    packLog.list.push.apply(packLog.list, JSON.parse(res).map(item => {
                        let arr = item.split(",");
                        let headAr = arr[0].split("=>");
                        let time = headAr[0].trim();
                        let message = headAr[1].trim();

                        let obj = null;
                        if (message.startsWith("打包成功")) {
                            obj = {
                                code: 1,
                                time: time,
                                message: message,
                                project: arr[1].split("=")[1],
                                package: arr[2].split("=")[1],
                                size: parseInt(arr[3].split("=")[1]) / 1024 / 1024,
                                sizeRate: 0,
                            };
                            obj.sizeRate = obj.size / that.packSizeMax > 1 ? 100 : obj.size / that.packSizeMax < 0 ? 0 : obj.size / that.packSizeMax * 100;
                        } else {
                            obj = {
                                code: 0,
                                time: time,
                                message: message,
                                project: arr[1].split("=")[1],
                                package: arr[2].split("=")[1],
                            };
                        }

                        return obj;
                    }));

                    packLog.isreport = false;
                    packLog.show = true;
                });
            },
            hideLogs() {
                app.hideLoading();
                this.packLog.show = false;
            },
        },
    }
</script>

<style>
.packlog{padding:1rem; border-bottom:1px solid #dee2e6;}
.packlog:last-child{border:none;}
.packlog_time{margin-bottom:.375rem; color:#0d6efd;}
.packlog_txt{display:flex; font-size:.85rem; word-break:break-all; word-wrap:break-word;}
.packlog_txt>span:first-child{flex-shrink:0; width:5rem; color:#6c757d;}
.packlog_txt>span:last-child{width:100%;}
.packlog_txt.error>span:last-child{color:red;}
.packlog_txt.success>span:last-child{color:green;}
.packlog_size{display:flex; align-items:center; margin-top:.5rem;}
.packlog_size_txt{flex-shrink:0; width:6rem; font-size:.85rem; text-align:right; color:#495057;}

.packlogfile{margin:0 .5rem .5rem 0; padding:.5rem 1rem; background:rgba(255,193,7, .2); border-radius:.25rem; cursor:pointer;}
.packlogfile_name{margin-bottom:0; font-weight:700; font-size:1.15rem;}
.packlogfile_lines{color:#495057;}

@media (max-width:767px) {
    .packlogfile{width:calc(100% / 2 - .5rem + .5rem / 2);}
    .packlogfile:nth-child(2n){margin-right:0;}
}
@media (min-width:768px) and (max-width:991px) {
    .packlogfile{width:calc(100% / 3 - .5rem + .5rem / 3);}
    .packlogfile:nth-child(3n){margin-right:0;}
}
@media (min-width:992px) {
    .packlogfile{width:calc(100% / 4 - .5rem + .5rem / 4);}
    .packlogfile:nth-child(4n){margin-right:0;}
}
</style>
