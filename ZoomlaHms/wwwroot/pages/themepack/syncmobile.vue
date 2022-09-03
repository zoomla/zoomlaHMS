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
                    <button class="btn btn-outline-secondary" v-on:click="openFile(config.themePackFile)"><i class="zi zi_externalLinkalt"></i></button>
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
                <button class="btn btn-info" v-on:click="pushFile"><i class="zi zi_syncalt"></i>推送文件</button>
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
                cmd: "",
                running: false,
                executing: false,
                timer: null,
                isdev: false,
                config: {
                    themePackFile: "",
                    mobileDirectory: "",
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
        },
        mounted() {
            const that = this;
            cscSetup("PushPakToMobile");

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
                                    CSharp.prompt("未连接设备");
                                    return;
                                }
                            } else {
                                cmd = "connect";
                            }
                            break;
                        case 3:
                            cmd = "0";
                            break;
                        case 4:
                            if (resp) {
                                if (resp.indexOf("No storage media found") > -1) {
                                    app.hideLoading();
                                    CSharp.prompt("请调整手机USB连接方式为：传输文件");
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
                                    CSharp.prompt("自动检测推送路径失败，连接的设备是否为华为系列手机？");
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
                        }, 500);
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
                                CSharp.prompt("未选择要推送的主题包");
                                break;
                            case "-11":
                                CSharp.prompt("主题包已被删除或移动");
                                break;
                            case "-1000":
                            case "-1001":
                                CSharp.prompt("推送路径格式错误");
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
                                    CSharp.prompt("未连接设备");
                                    return;
                                }
                            } else {
                                cmd = "connect";
                            }
                            break;
                        case 3:
                            cmd = "0";
                            break;
                        case 4:
                            if (resp) {
                                if (resp.indexOf("No storage media found") > -1) {
                                    app.hideLoading();
                                    CSharp.prompt("请调整手机USB连接方式为：传输文件");
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
                                    CSharp.prompt("自动检测推送路径失败，连接的设备是否为华为系列手机？");
                                    CSharp.prompt("你可以手动填写推送路径以绕过限制，注意路径以“/”开头以及结尾");
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
                                if (resp.indexOf("File creation on device failed") > -1) {
                                    CSharp.prompt("无法写入文件，推送失败");
                                    return;
                                }
                                if (resp.indexOf("File already exists") == -1) {
                                    app.hideLoading();
                                    CSharp.prompt("推送成功");
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
                                CSharp.prompt("推送成功");
                            } else {
                                let name = that.config.themePackFile.replace(/\\/g, "/");
                                name = name.substring(name.lastIndexOf("/") + 1);
                                cmd = "push \"" + that.config.mobileDirectory + name + "\" \"" + that.config.themePackFile + "\"";
                            }
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
                        }, 500);
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
</style>
