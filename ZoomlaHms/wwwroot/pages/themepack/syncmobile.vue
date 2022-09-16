<template>
    <div class="page" v-on:click="pathsel.show = false">
        <!-- 命令界面 -->
        <div class="conspane" v-if="isdev">
            <a class="conspane_stbtn" href="javascript:;" style="right:calc(4em + .5rem * 2);" v-on:click="isdev = false">设计者模式</a>
            <a class="conspane_stbtn stop" href="javascript:;" v-on:click="closeTool" v-if="running">关闭工具</a>
            <a class="conspane_stbtn start" href="javascript:;" v-on:click="openTool" v-else>启动工具</a>
            <div class="conspane_view" ref="history">
                <p v-for="item in history" v-bind:key="item" v-bind:class="item.startsWith('#>') ? 'mark' : ''">{{item}}</p>
            </div>
            <div class="conspane_cmdin">
                <input v-model="cmd" placeholder="输入指令，回车执行" v-on:keydown="execCommand" />
                <button v-on:click="execCommand">执行</button>
                <div class="conspane_cmdifst">
                    <div v-if="cmdFast.viewList.length" style="padding:.15rem .375rem; color:#adb5bd; font-size:.7rem;">Tips：按键盘上下方向键选择指令，再按回车即可键入指令</div>
                    <div class="conspane_cmdifst_item" v-for="item in cmdFast.viewList" v-bind:key="item" v-bind:class="item.selected ? 'active' : ''">
                        <label>{{item.command}}</label>
                        <span>{{item.label}}</span>
                    </div>
                </div>
            </div>
            <div class="conspane_help"></div>
        </div>

        <!-- 友好界面 -->
        <div class="fastpane" v-else>
            <div class="mb-3">
                <label class="form_label">主题包文件</label>
                <div class="input-group">
                    <input v-model="config.themePackFile" class="form-control" />
                    <button class="btn btn-outline-secondary" v-on:click="choosePakFile"><i class="zi zi_floderOpen"></i></button>
                    <button class="btn btn-outline-secondary" v-on:click="openFile(config.themePackFile)"><i class="zi zi_eye"></i></button>
                </div>
                <small class="text-danger">主题包文件路径中不能有中文</small>
            </div>
            <div class="mb-3">
                <label class="form_label">手机推送路径</label>
                <div class="input-group">
                    <div class="dronipt">
                        <input v-model="config.mobileDirectory" class="form-control" />
                        <span class="dronipt_btn" v-on:click.stop="pathsel.show = true"><i class="zi zi_forDown" style="margin-top:.1rem;"></i></span>
                        <div class="dronipt_list" v-bind:class="pathsel.show ? 'open' : ''">
                            <div class="dronipt_list_item" v-for="item in pathsel.list" v-bind:key="item" v-on:click="config.mobileDirectory = item.path">
                                <label>{{item.device}}：</label>
                                <span>{{item.path}}</span>
                            </div>
                        </div>
                    </div>
                    <button class="btn btn-outline-secondary" v-on:click="getMobilePath">自动检测</button>
                </div>
                <small class="text-muted">自动检测无效时可手动填写，格式示例：/Huawei/Themes/</small>
                <small class="text-danger">（推送路径中同样不能有中文）</small>
            </div>
            <div class="text-center position-relative">
                <small class="text-muted" style="position:absolute; top:calc(50% - 1em / 2 - 1px); left:calc(50% - 17rem);" v-if="lastExec">最后推送：{{lastExec}}</small>
                <button class="btn btn-info" v-on:click="pushFile"><i class="zi zi_syncalt"></i>推送文件</button>
                <a href="javascript:;" class="small link-secondary fst-italic" style="position:absolute; bottom:0; right:6rem;" v-on:click="showLogs('now')">查看推送日志</a>
                <a href="javascript:;" class="small link-secondary fst-italic" style="position:absolute; bottom:0; right:0;" v-on:click="isdev = true">开发者模式</a>
            </div>

            <div style="transform:translateY(50px); color:#343a40;">
                <div>本工具支持华为与荣耀机型的主题包推送，以下为使用步骤：</div>
                <div>1、将你的手机连接到电脑。连接后手机会提示选择连接模式，请选择“传输文件”模式。如无提示则查看手机通知栏，点击“USB连接选项”即可弹出提示。</div>
                <div>2、点击“主题包文件”下输入框后的文件夹按钮（输入框后第一个按钮），选择相应的 .hwt 文件。或者在输入框中直接输入路径。</div>
                <div>3、点击“手机推送路径”下输入框后的箭头按钮，选择对应的机型。或者点击后方“自动检测”按钮自动补全此字段。</div>
                <div>4、点击最下方“推送文件”按钮推送主题包到手机。</div>
                <div>5、在手机上打开“主题”APP，依次选择“我的”、“下载的主题”，找到你开发的主题，点击进入详情，最后点击主题详情下方的“应用”按钮即可。</div>
            </div>
        </div>

        <!-- 选择设备 -->
        <div class="modal" v-bind:class="deviceSel.show ? 'd-block' : ''">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">选择设备</h5>
                    </div>
                    <div class="modal-body">
                        <label class="form-check" v-for="item in deviceSel.list" v-bind:key="item">
                            <input type="radio" class="form-check-input" v-bind:value="item.id" v-model="deviceSel.sel" />
                            <span class="form-check-label">{{item.device}}</span>
                        </label>
                        <div class="mt-3 d-flex">
                            <div class="btn-group m-auto">
                                <a href="javascript:;" class="btn btn-danger" v-on:click="deviceSelCancel">取消</a>
                                <a href="javascript:;" class="btn btn-success" v-on:click="deviceSelConfirm">确定</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- 推送日志 -->
        <div class="modal modal-lg d-block" v-bind:class="pushLog.isreport ? 'modal-xl' : 'modal-lg'" v-if="pushLog.show">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content" v-if="!pushLog.isreport">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <span>{{pushLog.time}} 总计推送{{pushLog.list.length}}次</span>
                            <a href="javascript:;" class="ms-3" style="font-size:.85rem;" v-on:click="showLogFiles">历史记录</a>
                        </h5>
                        <a href="javascript:;" class="btn-close" v-on:click="hideLogs"></a>
                    </div>
                    <div class="modal-body p-0" style="min-height:40vh;">
                        <div class="pushlog" v-for="item in pushLog.list" v-bind:key="item">
                            <div class="pushlog_time">{{item.time}}</div>
                            <div class="pushlog_txt" v-bind:class="item.code ? 'success' : 'error'">
                                <span>推送状态：</span>
                                <span v-if="item.code">成功</span>
                                <span v-else>失败</span>
                            </div>
                            <div class="pushlog_txt">
                                <span>文件路径：</span>
                                <span>{{item.source}}</span>
                            </div>
                            <div class="pushlog_txt">
                                <span>手机路径：</span>
                                <span>{{item.target}}</span>
                            </div>
                            <div class="pushlog_txt">
                                <span>详细信息：</span>
                                <span>{{item.message}}</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-content" v-else>
                    <div class="modal-header">
                        <h5 class="modal-title">近30天推送记录</h5>
                        <a href="javascript:;" class="btn-close" v-on:click="hideLogs"></a>
                    </div>
                    <div class="modal-body" style="min-height:40vh;">
                        <div class="d-flex flex-wrap align-items-start justify-content-start">
                            <div class="pushlogfile" v-for="item in pushLog.reports" v-bind:key="item" v-on:click="showLogs(item.item1)"
                                v-bind:style="{'background':'rgba(255,193,7, ' + (0.2 + 0.6 * item.rate) + ')'}">
                                <p class="pushlogfile_name">{{item.item1}}</p>
                                <span class="pushlogfile_lines">推送次数：{{item.item2}}</span>
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
                pathsel: {
                    show: false,
                    list: [
                        { device: "荣耀机型", path: "/Honor/Themes/", },
                        { device: "华为机型", path: "/Huawei/Themes/", },
                    ],
                },
                cmdFast: {
                    show: false,
                    list: [
                        { command: "help", label: "显示简单使用帮助", },
                        { command: "start", label: "启动工具", },
                        { command: "stop", label: "关闭工具", },
                        { command: "clear", label: "清空控制台", },
                        { command: "connect", label: "连接设备，执行后需再按提示输入设备编号", },
                        { command: "list", label: "查看所有已连接的设备", },
                        { command: "disconnect", label: "断开与某个设备的连接，格式：disconnect <设备名称>", },
                        { command: "do", label: "执行外部命令，格式：do <命令>", },
                        { command: "map", label: "挂载设备储存空间（需先执行connect），执行后需再按提示输入设备编号", },
                        { command: "newfile", label: "在设备上创建文件，格式：newfile <手机文件路径> <电脑文件路径>", },
                        { command: "push", label: "推送文件到设备（需先执行newfile），格式：push <手机文件路径> <电脑文件路径>", },
                        { command: "pull", label: "从设备上获取文件", },
                        { command: "delete", label: "删除设备上的文件或文件夹，格式：delete <手机文件或文件夹路径>", },
                        { command: "dir", label: "列出设备上某个目录下的文件与文件夹，格式：dir <手机目录路径>", },
                        { command: "newdir", label: "在设备上创建文件夹，格式：newdir <手机目录路径>", },
                        // { command: "", label: "", },
                    ],
                    viewList: [],
                },

                history: [],
                timer: null,
                cmd: "",
                running: false,
                executing: false,

                lastExec: "",
                isdev: false,
                config: {
                    themePackFile: "",
                    mobileDirectory: "",
                },
                deviceSel: {
                    show: false,
                    list: [
                        //{ device: "", id: 0 },
                    ],
                    sel: null,
                    callback: null,
                    failback: null,
                },

                pushLog: {
                    show: false,
                    time: "",
                    list: [],

                    isreport: false,
                    reports: [],
                },
            };
        },
        watch: {
            "config": {
                deep: true,
                handler: function(n, o) {
                    csc("SetPakFile", n.themePackFile);
                    csc("SetMobileDirectory", n.mobileDirectory);
                }
            },
            "cmd": function(n ,e) {
                const cmdH = this.cmdFast;
                cmdH.viewList.splice(0, cmdH.viewList.length);
                if (!n) {
                    cmdH.show = false;
                    return;
                }

                var list = cmdH.list.filter(curr => curr.command.startsWith(n));
                if (!list.length) {
                    cmdH.show = false;
                    return;
                }

                cmdH.viewList.push.apply(cmdH.viewList, JSON.parse(JSON.stringify(list)).map(item => {
                    item.selected = false;
                    return item;
                }));
                cmdH.show = true;
            },
            "lastExec": function(n ,o) {
                csc("SetLastPushTime", n);
            },
        },
        mounted() {
            const that = this;
            cscSetup("PushPakToMobile");

            csc("GetLastPushTime").then(res => {
                if (res.startsWith("0")) {
                    return;
                }

                that.lastExec = res;
            });

            csc("IsRunning").then(res => {
                that.running = res == "1";
            });
            this.initPath();

            this.timer = setInterval(() => {
                csc("IsRunning").then(res => {
                    that.running = res == "1";
                    if (that.executing && !that.running) {
                        that.executing = false;
                    }
                });

                csc("GetNewestHistory").then(res => {
                    that.pushHistory(res);
                });
            }, 20);

            csc("GetConfig").then(res => {
                try {
                    let data = JSON.parse(res);
                    that.config.themePackFile = data.themePackFile;
                    that.config.mobileDirectory = data.mobileDirectory;
                } catch (error) { }
            });
        },
        beforeUnmount() {
            clearInterval(this.timer);
        },
        methods: {
            pushHistory(res) {
                const that = this;
                try {
                    let data;
                    if (typeof res == "string") {
                        data = JSON.parse(res);
                    } else {
                        data = res;
                    }

                    if (!data.length) {
                        return;
                    }

                    this.history.push.apply(this.history, data.map(item => {
                        if (item.startsWith("MtpAccess>")) {
                            return item.substring(10);
                        }
                        if (item.indexOf("]>") > -1) {
                            return item.substring(item.indexOf("]>") + 2);
                        }
                        return item;
                    }));

                    setTimeout(() => {
                        that.$refs.history.scrollTo(0, that.$refs.history.scrollHeight);
                    }, 200);
                } catch (error) { }
            },
            openTool() {
                const that = this;
                csc("StartProc").then(res => {
                    that.running = true;
                    that.history.splice(0, that.history.length);
                });
            },
            closeTool() {
                const that = this;
                csc("StopProc").then(res => {
                    that.running = false;
                });
            },
            execCommand(ev) {
                const that = this;
                if (!this.cmd || !this.cmd.trim()) {
                    return;
                }

                if (ev && ev.keyCode && ev.keyCode != 13) {
                    const cmdHl = this.cmdFast.viewList;
                    if (ev.keyCode == 38) {
                        //上
                        let idx = cmdHl.findIndex(curr => curr.selected);
                        if (idx == -1 || idx - 1 < 0)
                        { idx = cmdHl.length - 1; }
                        else
                        { idx -= 1; }

                        cmdHl.forEach(item => {
                            item.selected = false;
                        });
                        cmdHl[idx].selected = true;

                        ev.preventDefault();
                    } else if (ev.keyCode == 40) {
                        //下
                        let idx = cmdHl.findIndex(curr => curr.selected);
                        if (idx == -1 || idx + 1 >= cmdHl.length)
                        { idx = 0; }
                        else
                        { idx += 1; }

                        cmdHl.forEach(item => {
                            item.selected = false;
                        });
                        cmdHl[idx].selected = true;

                        ev.preventDefault();
                    } else if (ev.keyCode == 9) {
                        ev.preventDefault();
                    }
                    return;
                }
                if (ev && ev.keyCode && ev.keyCode == 13) {
                    let idx = this.cmdFast.viewList.findIndex(curr => curr.selected);
                    if (idx > -1) {
                        this.cmd = this.cmdFast.viewList[idx].command + " ";
                        return;
                    }
                }

                if (this.cmd.trim() == "start") {
                    this.openTool();
                    this.cmd = "";
                    return;
                }
                if (this.cmd.trim() == "stop") {
                    this.closeTool();
                    this.cmd = "";
                    return;
                }
                if (this.cmd.trim() == "clear") {
                    this.history.splice(0, this.history.length);
                    this.cmd = "";
                    return;
                }
                if (this.cmd.trim() == "help") {
                    this.history.push("#>" + this.cmd);

                    this.history.push("start 启动工具");
                    this.history.push("stop 关闭工具");
                    this.history.push("clear 清空控制台");
                    this.history.push("help 再次显示帮助");
                    this.history.push("常用连接命令：");
                    this.history.push("connect 连接新设备，执行后需再按提示输入设备编号");
                    this.history.push("map 挂载设备储存空间（需先执行connect），执行后需再按提示输入设备编号");
                    this.history.push("设备挂载好后可用：");
                    this.history.push("newfile 在设备上创建文件，格式：newfile <手机文件路径> <电脑文件路径>");
                    this.history.push("push 推送文件到设备（需先执行newfile），格式：push <手机文件路径> <电脑文件路径>");
                    this.history.push("delete 删除设备上的文件，格式：delete <手机文件路径>");
                    this.history.push("dir 列出设备上某个目录下的文件与文件夹，格式：dir <手机目录路径>");
                    this.history.push("newdir 在设备上创建文件夹，格式：newdir <手机目录路径>");

                    this.cmd = "";
                    return;
                }

                if (!this.running || this.executing) {
                    return;
                }

                this.history.push("#>" + this.cmd);
                this.$nextTick(function() {
                    that.$refs.history.scrollTo(0, that.$refs.history.scrollHeight);
                });
                this.executing = true;
                csc("ExecuteCommand", this.cmd + "::string").then(res => {
                    that.cmd = "";
                    setTimeout(function() {
                        that.executing = false;
                    }, 150);
                });
            },
            back() {
                app.hideLoading();
                app.navigateBack();
            },
            openFile(path) {
                CSharp.openFile(path);
            },



            initPath() {
                const that = this;
                csc("GetPakFile").then(res => {
                    that.config.themePackFile = res;
                });
                csc("GetMobileDirectory").then(res => {
                    that.config.mobileDirectory = res;
                });
            },
            choosePakFile() {
                const that = this;
                csc("ChoosePakFile").then(res => {
                    that.config.themePackFile = res;
                });
            },
            chooseMobilePath() {
                const that = this;
                csc("ChooseMobileDirectory").then(res => {
                    that.config.mobileDirectory = res;
                });
            },
            getMobilePath() {
                if (this.executing) {
                    return;
                }

                app.showLoading();
                if (!this.running) {
                    this.openTool();
                }

                const that = this;
                var timer = null;
                const _step = function(step, resp) {
                    let cmd = "";
                    switch (step) {
                        case 1:
                            if (resp) {
                                if (resp.indexOf("newfile") > -1) {
                                    timer = null;
                                    _step(6, null);
                                    return;
                                }
                            } else {
                                cmd = "newfile";
                            }
                            break;
                        case 2:
                            if (resp) {
                                if (resp.indexOf("No available devices") > -1) {
                                    app.hideLoading();
                                    CSharp.prompt("[一般性例外]未连接设备，请将你的手机连接到电脑。");
                                    return;
                                }

                                let harr = resp.split("\n");
                                if (harr.length == 2) {
                                    timer = null;
                                    _step(3, 0);
                                    return;
                                }

                                let json = [];
                                for (let i = 0; i < harr.length - 1; i++) {
                                    let hitem = harr[i];
                                    let idx = hitem.indexOf("]");
                                    json.push({
                                        id: hitem.substring(1, idx),
                                        device: hitem.substring(idx + 1)
                                    });
                                }

                                const deviceSel = that.deviceSel;
                                deviceSel.sel = null;
                                deviceSel.list.splice(0, deviceSel.list.length);
                                deviceSel.list.push.apply(deviceSel.list, json);
                                timer = null;
                                deviceSel.callback = function(selres) {
                                    _step(3, selres);
                                };
                                deviceSel.show = true;
                                return;
                            } else {
                                cmd = "connect";
                            }
                            break;
                        case 3:
                            cmd = resp;
                            break;
                        case 4:
                            if (resp) {
                                if (resp.indexOf("No storage media found") > -1) {
                                    app.hideLoading();
                                    CSharp.prompt("[一般性例外]请调整手机USB连接方式为：传输文件。");
                                    that.closeTool();
                                    return;
                                }
                            } else {
                                cmd = "map";
                            }
                            break;
                        case 5:
                            cmd = "0";
                            break;
                        case 6:
                            if (resp) {
                                if (resp.indexOf("This path is not a valid directory") == -1) {
                                    that.config.mobileDirectory = "/Honor/Themes/";
                                    csc("SetMobileDirectory", "/Honor/Themes/");
                                    timer = null;
                                    _step(8, null);
                                    return;
                                }
                            } else {
                                cmd = "dir /Honor/Themes/";
                            }
                            break;
                        case 7:
                            if (resp) {
                                if (resp.indexOf("This path is not a valid directory") == -1) {
                                    that.config.mobileDirectory = "/Huawei/Themes/";
                                    csc("SetMobileDirectory", "/Huawei/Themes/");
                                } else {
                                    CSharp.prompt("[一般性例外]自动检测推送路径失败，连接的设备是否为华为系列手机？");
                                }
                            } else {
                                cmd = "dir /Huawei/Themes/";
                            }
                            break;
                        case 8:
                            cmd = "dir";
                            break;
                        default:
                            app.hideLoading();
                            return;
                    }

                    if (!timer) {
                        that.cmd = cmd;
                        that.execCommand();
                        timer = setInterval(() => {
                            if (that.executing) {
                                return;
                            }

                            clearInterval(timer);
                            let idx = that.history.findLastIndex(curr => curr == "#>" + cmd);
                            _step(step, that.history.slice(idx + 1, that.history.length).join("\n") || " ");
                        }, 650);
                    } else {
                        timer = null;
                        _step(step + 1, null);
                    }
                };

                setTimeout(() => {
                    if (that.running) {
                        _step(1, null);
                    }
                }, 200);
            },
            pushFile(cfm) {
                if (this.executing) {
                    return;
                }

                const that = this;
                app.showLoading();
                if (!this.running) {
                    this.openTool();
                }

                if (typeof cfm != "boolean" || !cfm) {
                    csc("CheckPath").then(res => {
                        that.initPath();
                        if (res == "1") {
                            that.pushFile(true);
                            return;
                        }

                        switch (res.toString()) {
                            case "-10":
                                CSharp.prompt("[一般性例外]未选择要推送的主题包。");
                                csc("AddPushLog", 0, "为选择主题包");
                                break;
                            case "-11":
                                CSharp.prompt("[一般性例外]主题包已被删除或移动。");
                                csc("AddPushLog", 0, "主题包不存在");
                                break;
                            case "-1000":
                            case "-1001":
                                CSharp.prompt("[一般性例外]推送路径格式错误！");
                                csc("AddPushLog", 0, "推送路径格式错误");
                                break;
                            default:
                                break;
                        }
                        app.hideLoading();
                    });
                    return;
                }

                var timer = null;
                const _step = function(step, resp) {
                    let cmd = "";
                    switch (step) {
                        case 1:
                            if (resp) {
                                if (resp.indexOf("newfile") > -1) {
                                    timer = null;
                                    _step(6, null);
                                    return;
                                }
                            } else {
                                cmd = "newfile";
                            }
                            break;
                        case 2:
                            if (resp) {
                                if (resp.indexOf("No available devices") > -1) {
                                    app.hideLoading();
                                    CSharp.prompt("[一般性例外]未连接设备，请将你的手机连接到电脑。");
                                    csc("AddPushLog", 0, "未连接设备");
                                    return;
                                }

                                let harr = resp.split("\n");
                                // alert(harr.length)
                                if (harr.length == 2) {
                                    timer = null;
                                    _step(3, "0");
                                    return;
                                }

                                let json = [];
                                for (let i = 0; i < harr.length - 1; i++) {
                                    let hitem = harr[i];
                                    let idx = hitem.indexOf("]");
                                    json.push({
                                        id: hitem.substring(1, idx),
                                        device: hitem.substring(idx + 1)
                                    });
                                }

                                const deviceSel = that.deviceSel;
                                deviceSel.sel = null;
                                deviceSel.list.splice(0, deviceSel.list.length);
                                deviceSel.list.push.apply(deviceSel.list, json);
                                timer = null;
                                deviceSel.failback = function() {
                                    csc("AddPushLog", 0, "未选择推送设备。");
                                };
                                deviceSel.callback = function(selres) {
                                    _step(3, selres);
                                };
                                deviceSel.show = true;
                                return;
                            } else {
                                cmd = "connect";
                            }
                            break;
                        case 3:
                            cmd = resp;
                            break;
                        case 4:
                            if (resp) {
                                if (resp.indexOf("No storage media found") > -1) {
                                    app.hideLoading();
                                    CSharp.prompt("[一般性例外]请调整手机USB连接方式为：传输文件！");
                                    csc("AddPushLog", 0, "手机USB连接方式为调整为“传输文件”");
                                    that.closeTool();
                                    return;
                                }
                            } else {
                                cmd = "map";
                            }
                            break;
                        case 5:
                            cmd = "0";
                            break;
                        case 6:
                            if (resp) {
                                if (resp.indexOf("This path is not a valid directory") == -1) {
                                    that.config.mobileDirectory = "/Honor/Themes/";
                                    csc("SetMobileDirectory", "/Honor/Themes/");
                                    timer = null;
                                    _step(8, null);
                                    return;
                                }
                            } else {
                                if (!that.config.mobileDirectory) {
                                    cmd = "dir /Honor/Themes/";
                                } else {
                                    timer = null;
                                    _step(8, null);
                                    return;
                                }
                            }
                            break;
                        case 7:
                            if (resp) {
                                if (resp.indexOf("This path is not a valid directory") == -1) {
                                    that.config.mobileDirectory = "/Huawei/Themes/";
                                    csc("SetMobileDirectory", "/Huawei/Themes/");
                                } else {
                                    app.hideLoading();
                                    CSharp.prompt("[一般性例外]自动检测推送路径失败，连接的设备是否为华为系列手机？");
                                    CSharp.prompt("[普通消息]你可以手动填写推送路径以禁用自动检测，注意路径以“/”开头以及结尾。");
                                    csc("AddPushLog", 0, "因未填写推送路径，已触发路径检测逻辑。但自动检测未成功，连接的设备可能不是华为系列手机。");
                                    return;
                                }
                            } else {
                                cmd = "dir /Huawei/Themes/";
                            }
                            break;
                        case 8:
                            cmd = "newdir " + that.config.mobileDirectory;
                            break;
                        case 9:
                            if (resp) {
                                if (resp.indexOf("File already exists") > -1) {
                                    timer = null;
                                    _step(10, null);
                                    return;
                                }
                                if (resp.indexOf("File created successfully") > -1) {
                                    app.hideLoading();
                                    CSharp.prompt("[普通消息]推送成功，接下来只要在手机主题APP中应用主题即可测试效果。");
                                    that.lastExec = now.getFullYear() + "-"
                                        + (now.getMonth() + 1 < 10 ? "0" : "") + (now.getMonth() + 1) + "-"
                                        + (now.getDate() < 10 ? "0" : "") + now.getDate()
                                        + " "
                                        + (now.getHours() < 10 ? "0" : "") + now.getHours() + ":"
                                        + (now.getMinutes() < 10 ? "0" : "") + now.getMinutes() + ":"
                                        + (now.getSeconds() < 10 ? "0" : "") + now.getSeconds();
                                    csc("SetLastPushTime", that.lastExec);
                                    csc("AddPushLog", 1, "推送成功。");
                                    return;
                                } else {
                                    app.hideLoading();
                                    CSharp.prompt("[一般性例外]文件传送失败，请再次推送！");
                                    csc("AddPushLog", 0, "文件创建失败。");
                                    setTimeout(() => {
                                        that.closeTool();
                                    }, 500);
                                    return;
                                }
                            } else {
                                let name = that.config.themePackFile.replace(/\\/g, "/");
                                name = name.substring(name.lastIndexOf("/") + 1);
                                cmd = "newfile \"" + that.config.mobileDirectory + name + "\" \"" + that.config.themePackFile + "\"";
                            }
                            break;
                        case 10:
                            if (resp) {
                                if (resp.indexOf("File write successful") > -1) {
                                    CSharp.prompt("[普通消息]推送成功，接下来只要在手机主题APP中应用主题即可测试效果。");
                                    csc("AddPushLog", 1, "推送成功。");
                                } else {
                                    CSharp.prompt("[一般性例外]文件传送失败，请再次推送！");
                                    csc("AddPushLog", 0, "文件写入失败。");
                                    setTimeout(() => {
                                        that.closeTool();
                                    }, 500);
                                }
                            } else {
                                let name = that.config.themePackFile.replace(/\\/g, "/");
                                name = name.substring(name.lastIndexOf("/") + 1);
                                cmd = "push \"" + that.config.mobileDirectory + name + "\" \"" + that.config.themePackFile + "\"";
                            }
                            break;
                        default:
                            app.hideLoading();
                            var now = new Date();
                            that.lastExec = now.getFullYear() + "-"
                                + (now.getMonth() + 1 < 10 ? "0" : "") + (now.getMonth() + 1) + "-"
                                + (now.getDate() < 10 ? "0" : "") + now.getDate()
                                + " "
                                + (now.getHours() < 10 ? "0" : "") + now.getHours() + ":"
                                + (now.getMinutes() < 10 ? "0" : "") + now.getMinutes() + ":"
                                + (now.getSeconds() < 10 ? "0" : "") + now.getSeconds();
                            csc("SetLastPushTime", that.lastExec);
                            return;
                    }

                    if (!timer) {
                        that.cmd = cmd;
                        that.execCommand();
                        timer = setInterval(() => {
                            if (that.executing) {
                                return;
                            }

                            let idx = that.history.findLastIndex(curr => curr == "#>" + cmd);
                            if (cmd.startsWith("dir")) {
                                clearInterval(timer);
                                _step(step, that.history.slice(idx + 1, that.history.length).join("\n") || " ");
                            } else {
                                let ar = that.history.slice(idx + 1, that.history.length);
                                if (!ar.length) {
                                    return;
                                }

                                clearInterval(timer);
                                _step(step, ar.join("\n") || " ");
                            }
                        }, 650);
                    } else {
                        timer = null;
                        _step(step + 1, null);
                    }
                };

                setTimeout(() => {
                    if (that.running) {
                        _step(1, null);
                    }
                }, 200);
            },

            deviceSelCancel() {
                const deviceSel = this.deviceSel;
                deviceSel.show = false;
                deviceSel.callback = null;
                app.hideLoading();

                this.cmd = "-1";
                this.execCommand();

                if (deviceSel.failback) {
                    deviceSel.failback();
                }
            },
            deviceSelConfirm() {
                const deviceSel = this.deviceSel;
                if (deviceSel.sel === null) {
                    CSharp.prompt("[一般性例外]请选择设备");
                    return;
                }

                deviceSel.show = false;
                if (deviceSel.callback) {
                    deviceSel.callback(deviceSel.sel.toString());
                    deviceSel.callback = null;
                }
            },

            showLogFiles() {
                const that = this;
                this.pushLog.show = false;
                app.showLoading();

                csc("GetPushLogFileList").then(res => {
                    const pushLog = that.pushLog;

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

                    pushLog.reports.splice(0, pushLog.reports.length);
                    pushLog.reports.push.apply(pushLog.reports, arr);

                    pushLog.isreport = true;
                    pushLog.show = true;
                });
            },
            showLogs(time) {
                const that = this;
                this.pushLog.show = false;
                app.showLoading();

                var now = new Date();
                now.setMilliseconds(0);
                now.setSeconds(0);
                now.setMinutes(0);
                now.setHours(8);

                if (time == "now" || time == "今日" || Date.parse(time) == now.getTime()) {
                    this.pushLog.time = "今日";
                } else {
                    this.pushLog.time = time;
                }
                if (time == "今日") {
                    time = "now";
                }

                csc("GetPushLog", time + "::string").then(res => {
                    const pushLog = that.pushLog;

                    pushLog.list.splice(0, pushLog.list.length);
                    pushLog.list.push.apply(pushLog.list, JSON.parse(res).map(item => {
                        let arr = item.split(",");
                        let headAr = arr[0].split("=>");
                        let time = headAr[0].trim();
                        let message = headAr[1].trim();

                        let obj;
                        if (message.startsWith("推送成功")) {
                            obj = {
                                code: 1,
                                time: time,
                                message: message,
                                source: arr[1].split("=")[1],
                                target: arr[2].split("=")[1],
                            };
                        } else {
                            obj = {
                                code: 0,
                                time: time,
                                message: message,
                                source: arr[1].split("=")[1],
                                target: arr[2].split("=")[1],
                            };
                        }

                        return obj;
                    }));

                    pushLog.isreport = false;
                    pushLog.show = true;
                });
            },
            hideLogs() {
                app.hideLoading();
                this.pushLog.show = false;
            },
        },
    }
</script>

<style>
.page{display:flex; flex-flow:column; width:100%; height:100%; position:relative;}
.page_header~.conspane{height:calc(100% - 3rem);}
.page_header~.fastpane{height:calc(100% - 3rem);}

.conspane{display:flex; flex-flow:column; height:100%; position:relative; background:#012456; overflow:hidden;}
.conspane_stbtn{display:block; padding:.375rem .5rem; position:absolute; top:0; right:0; font-size:.85rem; color:#fff; background:#adb5bd; text-decoration:none;}
.conspane_stbtn.start{background:#198754;}
.conspane_stbtn.stop{background:#dc3534;}
.conspane_view{height:calc(100% - 2.25rem); padding:.5rem; color:#fff; overflow:auto;}
.conspane_view>p{margin-bottom:0; background:transparent;}
.conspane_view>p.mark{color:#ffff00;}
.conspane_cmdin{flex-shrink:0; display:flex; align-items:stretch; height:2.25rem; position:relative;}
.conspane_cmdin>input{flex:1; display:inline-block; width:100%; padding:0 .5rem; color:#fff; background:rgba(255,255,255, .375); border:none; outline:none;}
.conspane_cmdin>input::placeholder{color:#ced4da;}
.conspane_cmdin>button{flex-shrink:0; display:inline-block; padding:0 .65rem; background:#ffc107; border:none; outline:none;}

.conspane_cmdifst{width:100%; position:absolute; left:0; bottom:100%; z-index:3; color:#f3f4f6; background:rgba(50,50,50, .9);}
.conspane_cmdifst_item{display:flex; align-items:flex-end; padding:.375rem .5rem;}
.conspane_cmdifst_item>span{margin-left:.5rem; font-size:.7rem; color:#dee2e6; font-style:italic;}
.conspane_cmdifst_item.active{background:rgba(255,255,255, .2);}

.fastpane{height:100%;}
.fastpane.hide{opacity:0; pointer-events:none;}

.dronipt{flex:1; position:relative;}
.dronipt>.form-control{padding-right:calc(.75rem + 3rem); border-top-right-radius:0; border-bottom-right-radius:0;}
.dronipt_btn{display:flex; justify-content:center; align-items:center; width:3rem; height:calc(100% - 2px); position:absolute; top:1px; right:1px; cursor:pointer;}
.dronipt_btn:hover{background:#f8f9fa;}
.dronipt_btn:active{background:#e9ecef;}
.dronipt_list{display:none; width:100%; position:absolute; left:0; top:100%; z-index:3; background:#f8f9fa; border-width:0 1px 1px 1px; border-style:solid; border-radius:0 0 .375rem .375rem; border-color:#dee2e6;}
.dronipt_list.open{display:block;}
.dronipt_list_item{padding:.5rem 1rem; border-bottom:1px solid #dee2e6; cursor:pointer;}
.dronipt_list_item:last-child{border-bottom:0;}
.dronipt_list_item:hover{background:#e9ecef;}
.dronipt_list_item:active{background:#dee2e6;}
.dronipt_list_item>label{color:#0d6efd;}

.pushlog{padding:1rem; border-bottom:1px solid #dee2e6;}
.pushlog:last-child{border:none;}
.pushlog_time{margin-bottom:.375rem; color:#0d6efd;}
.pushlog_txt{display:flex; font-size:.85rem; word-break:break-all; word-wrap:break-word;}
.pushlog_txt>span:first-child{flex-shrink:0; width:5rem; color:#6c757d;}
.pushlog_txt>span:last-child{width:100%;}
.pushlog_txt.success>span:last-child{color:green;}
.pushlog_txt.error>span:last-child{color:red;}

.pushlogfile{margin:0 .5rem .5rem 0; padding:.5rem 1rem; background:rgba(255,193,7, .2); border-radius:.25rem; cursor:pointer;}
.pushlogfile_name{margin-bottom:0; font-weight:700; font-size:1.15rem;}
.pushlogfile_lines{color:#495057;}

@media (max-width:767px) {
    .pushlogfile{width:calc(100% / 2 - .5rem + .5rem / 2);}
    .pushlogfile:nth-child(2n){margin-right:0;}
}
@media (min-width:768px) and (max-width:991px) {
    .pushlogfile{width:calc(100% / 3 - .5rem + .5rem / 3);}
    .pushlogfile:nth-child(3n){margin-right:0;}
}
@media (min-width:992px) {
    .pushlogfile{width:calc(100% / 4 - .5rem + .5rem / 4);}
    .pushlogfile:nth-child(4n){margin-right:0;}
}
</style>
