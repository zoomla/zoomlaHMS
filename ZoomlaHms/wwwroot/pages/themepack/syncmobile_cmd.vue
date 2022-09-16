<template>
    <div class="page">
        <div class="clspane_main">
            <div ref="history" class="clspane_history">
                <div v-for="item in history" v-bind:key="item">
                    <p v-bind:class="item.startsWith('#>') ? 'mark' : ''" v-if="item && item != '#>'">{{item}}</p>
                </div>
            </div>
            <div class="clspane_help_toggle" v-bind:class="help.show ? 'active' : ''" v-on:click="help.show=!help.show">
                <i class="zi zi_forLeft" v-if="!help.show"></i>
                <i class="zi zi_forRight" v-else></i>
            </div>
            <div class="clspane_help" v-bind:class="help.show ? 'show' : ''">
                <div class="clspane_help_tips">
                    Tips #1: 点击左侧文本输入命令，点击右侧图标展开命令说明。<br />
                    Tips #2: 下方列表可以滚动，其滚动条已被隐藏。
                </div>
                <div class="clspane_help_list">
                    <div class="clspane_help_litem" v-for="item in help.list" v-bind:key="item">
                        <div class="clspane_help_litem_head">
                            <span class="clspane_help_litem_link" v-on:click="applyHelp(item)">{{item.name}}</span>
                            <span class="clspane_help_litem_sdesc" v-on:click="showHelpDetail(item)">
                                <i class="zi zi_forDown" v-if="!item.showHelp"></i>
                                <i class="zi zi_forUp" v-else></i>
                            </span>
                        </div>
                        <div class="clspane_help_litem_body" v-if="item.showHelp">
                            <div class="clspane_help_litem_htxt">{{item.detail}}</div>
                            <div class="clspane_help_litem_abtn"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div ref="console" class="clspane_usrin">
            <textarea ref="console_text" class="clspane_usrin_input" v-model="cmd" v-on:keydown="evClsKeydown" v-on:input="evClsInput" placeholder="输入命令，回车执行"></textarea>
            <div ref="console_fake" class="clspane_usrin_infak">A{{cmd}}A</div>
        </div>
        <div class="runstate" v-bind:class="running ? 'open' : 'close'">
            <div v-if="!running">
                <i class="zi zi_pausecircle"></i>
                <span>已停止</span>
            </div>
            <div v-else>
                <i class="zi zi_playcircle"></i>
                <span>正在运行</span>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                help: {
                    show: true,
                    list: [
                        { name: "help", detail: "显示简单使用帮助。", showHelp: false, command: "help", },
                        { name: "start", detail: "启动工具。", showHelp: false, command: "start", },
                        { name: "stop", detail: "关闭工具。", showHelp: false, command: "stop", },
                        { name: "clear", detail: "清空控制台。", showHelp: false, command: "clear", },
                        { name: "connect", detail: "连接设备。\n执行后需再按提示输入设备编号。", showHelp: false, command: "connect", },
                        { name: "list", detail: "查看所有已连接的设备。", showHelp: false, command: "list", },
                        { name: "disconnect", detail: "断开与某个设备的连接。\n格式：disconnect <设备名称>", showHelp: false, command: "disconnect [设备名称]", },
                        { name: "do", detail: "执行外部命令。\n格式：do <命令>", showHelp: false, command: "do", },
                        { name: "map", detail: "挂载设备储存空间。\n执行后需再按提示输入设备编号。", showHelp: false, command: "map", },
                        { name: "newfile", detail: "在设备上创建文件。\n格式：newfile <手机文件路径> <电脑文件路径>", showHelp: false, command: "newfile \"[手机文件绝对路径]\" \"[电脑文件绝对路径]\"", },
                        { name: "push", detail: "推送文件到设备。\n格式：push <手机文件路径> <电脑文件路径>", showHelp: false, command: "push \"[手机文件绝对路径]\" \"[电脑文件绝对路径]\"", },
                        { name: "pull", detail: "从设备上获取文件", showHelp: false, command: "pull \"[手机文件绝对路径]\" \"[电脑文件绝对路径]\"", },
                        { name: "dir", detail: "列出设备上某个目录下的文件与文件夹。\n格式：dir <手机目录路径>", showHelp: false, command: "dir \"[手机文件夹绝对路径]\"", },
                        { name: "newdir", detail: "在设备上创建文件夹。\n格式：newdir <手机目录路径>", showHelp: false, command: "newdir \"[手机文件夹绝对路径]\"", },
                        { name: "delete", detail: "删除设备上的文件或文件夹。\n格式：delete <手机文件或文件夹路径>", showHelp: false, command: "delete \"[手机文件或文件夹绝对路径]\"", },
                    ],
                },
                history: [],
                inHistory: {
                    list: [],
                    index: -1,
                    hold: "",
                },
                timer: null,
                cmd: "",
                running: false,
                executing: false,
            };
        },
        mounted() {
            const that = this;
            cscSetup("PushPakToMobile");

            var scrolling = false;
            var scroll = function() {
                if (scrolling) {
                    return;
                }
                scrolling = true;

                setTimeout(() => {
                    scrolling = false;
                    that.$refs.history.scrollTo(0, that.$refs.history.scrollHeight);
                }, 200);
            };

            this.timer = setInterval(() => {
                csc("IsRunning").then(res => {
                    that.running = res == "1";
                    if (that.executing && !that.running) {
                        that.executing = false;
                    }
                });

                csc("GetHistory").then(res => {
                    let data;
                    if (typeof res == "string") {
                        data = JSON.parse(res);
                    } else {
                        data = res;
                    }

                    if (!data.length) {
                        return;
                    }

                    if (that.history.length && data.length) {
                        let newLast = data[data.length - 1];
                        let oldLast = that.history[that.history.length - 1];
                        if (newLast.startsWith("MtpAccess>")) {
                            newLast = newLast.substring(10);
                        }
                        if (newLast.indexOf("]>") > -1) {
                            newLast = newLast.substring(newLast.indexOf("]>") + 2);
                        }
                        if (newLast == oldLast) {
                            return;
                        }
                    }

                    that.history.splice(0, that.history.length);
                    that.history.push.apply(that.history, data.map(item => {
                        if (item.startsWith("MtpAccess>")) {
                            return "#>" + item.substring(10);
                        }
                        if (item.indexOf("]>") > -1) {
                            return "#>" + item.substring(item.indexOf("]>") + 2);
                        }
                        return item;
                    }));

                    scroll();
                });
            }, 20);
        },
        beforeUnmount() {
            clearInterval(this.timer);
        },
        methods: {
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
            execCommand() {
                const that = this;
                if (!this.cmd || !this.cmd.trim()) {
                    return;
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
                    this.history.push("dir 列出设备上某个目录下的文件与文件夹，格式：dir <手机目录路径>");
                    this.history.push("newdir 在设备上创建文件夹，格式：newdir <手机目录路径>");
                    this.history.push("delete 删除设备上的文件，格式：delete <手机文件路径>");

                    this.cmd = "";
                    return;
                }

                if (!this.running || this.executing) {
                    return;
                }

                this.$nextTick(function() {
                    that.$refs.history.scrollTo(0, that.$refs.history.scrollHeight);
                });
                this.executing = true;
                csc("ExecuteCommand", this.cmd + "::string").then(res => {
                    that.inHistory.list.push(that.cmd);
                    that.cmd = "";
                    setTimeout(function() {
                        that.executing = false;
                    }, 150);
                });
            },

            evClsKeydown(ev) {
                if (ev.keyCode == 38) {
                    const inHistory = this.inHistory;
                    if (this.$refs.console_text.selectionStart == 0) {
                        if (!inHistory.list.length) {
                            return;
                        }

                        if (inHistory.index == -1) {
                            inHistory.hold = this.cmd;
                            inHistory.index = inHistory.list.length - 1;
                        }
                        if (inHistory.index >= inHistory.list.length) {
                            inHistory.index = inHistory.list.length - 1;
                        }

                        this.cmd = inHistory.list[inHistory.index];
                        this.$nextTick(function() {
                            const $input = this.$refs.console;
                            const $fake = this.$refs.console_fake;

                            let height = Math.max(Math.floor($fake.scrollHeight / 24) * 24, 24);
                            if (height / 24 > 4) {
                                height = 24 * 4;
                            }
                            $input.style.height = "calc(.375rem * 2 + " + height + "px)";
                        });
                        if (inHistory.index > 0) {
                            inHistory.index = inHistory.index - 1;
                        }
                    }
                    return;
                }
                if (ev.keyCode == 40) {
                    const inHistory = this.inHistory;
                    if (this.$refs.console_text.selectionStart >= this.cmd.length) {
                        if (inHistory.index == -1) {
                            return;
                        }
                        if (inHistory.index >= inHistory.list.length) {
                            this.cmd = inHistory.hold;
                            return;
                        }

                        this.cmd = inHistory.list[inHistory.index];
                        this.$nextTick(function() {
                            const $input = this.$refs.console;
                            const $fake = this.$refs.console_fake;

                            let height = Math.max(Math.floor($fake.scrollHeight / 24) * 24, 24);
                            if (height / 24 > 4) {
                                height = 24 * 4;
                            }
                            $input.style.height = "calc(.375rem * 2 + " + height + "px)";
                        });
                        inHistory.index = inHistory.index + 1;
                    }
                    return;
                }

                if (ev.keyCode == 13) {
                    if (!ev.shiftKey) {
                        this.execCommand();
                    }
                    ev.preventDefault();
                    return;
                }
            },
            evClsInput(ev) {
                const $input = this.$refs.console;
                const $fake = this.$refs.console_fake;

                let height = Math.max(Math.floor($fake.scrollHeight / 24) * 24, 24);
                if (height / 24 > 4) {
                    height = 24 * 4;
                }
                $input.style.height = "calc(.375rem * 2 + " + height + "px)";

                this.inHistory.index = -1;
            },
            showHelpDetail(ev) {
                ev.showHelp = !ev.showHelp;
                const compare = JSON.stringify(ev);

                const list = this.help.list;
                for (let i = 0; i < list.length; i++) {
                    const item = list[i];
                    if (JSON.stringify(item) == compare) {
                        continue;
                    }

                    item.showHelp = false;
                }
            },
            applyHelp(ev) {
                this.cmd = ev.command;
                this.$refs.console_text.focus();
            },
            reload() {
                location.reload();
            },
        },
    }
</script>

<style>
.page{display:flex; flex-flow:column; height:100%; padding:0; background:#012456;}
.runstate{width:6rem; height:1.65rem; padding:0 .5rem; position:fixed; top:0; left:calc(50% - 6rem / 2); z-index:9; color:#fff; background:#333; border-radius:0 0 .15rem .15rem;}
.runstate.open{background:#198754;}
.runstate.close{background:#dc3534;}
.runstate>div{display:flex; justify-content:center; align-items:center; height:100%; font-size:.85rem;}
.runstate>div>i{margin-right:.15rem;}

.clspane_main{flex:1; position:relative; overflow:hidden;}
.clspane_history{height:100%; padding:.5rem; color:#fff; overflow:auto;}
.clspane_history p{margin-bottom:0; background:transparent;}
.clspane_history p.mark{color:#ffff00;}
.clspane_help_toggle{display:flex; justify-content:center; align-items:center; width:1.65rem; height:3rem; position:absolute; right:0; top:calc(50% - 3rem / 2); color:#fff; background:#27456f; border-radius:.15rem 0 0 .15rem; cursor:pointer;}
.clspane_help_toggle.active{right:220px;}

.clspane_help{display:flex; flex-flow:column; width:220px; height:100%; position:absolute; top:0; right:-220px; z-index:3; color:#f8f9fa; background:#27456f;}
.clspane_help.show{right:0;}
.clspane_help_tips{padding:.15rem .375rem; color:#adb5bd; font-size:.7rem;}
.clspane_help_list{width:calc(100% + 17px); height:calc(100% - 4rem); margin-top:auto; background:#213b5e; overflow-y:auto;}
.clspane_help_litem{}
.clspane_help_litem_head{display:flex; padding:.5rem; justify-content:space-between;}
.clspane_help_litem_link{display:block; text-decoration:underline; cursor:pointer;}
.clspane_help_litem_sdesc{flex:1; display:block; text-align:right; cursor:pointer;}
.clspane_help_litem_body{padding:.5rem; font-size:.85rem; color:#dee2e6; background:rgba(0,0,0, .15);}
.clspane_help_litem_htxt{white-space:pre-line;}

.clspane_usrin{height:calc(24px + .375rem * 2); padding:.375rem .5rem; position:relative; background:#617696;}
.clspane_usrin_input{display:block; width:100%; height:100%; padding:0; color:#fff; background:transparent; border:0; outline:none; resize:none; overflow:auto; word-break:break-all; word-wrap:break-word;}
.clspane_usrin_input::placeholder{color:#ced4da;}
.clspane_usrin_infak{width:100%; padding:.375rem .5rem; position:fixed; bottom:-1000px; left:0; white-space:pre-line; word-break:break-all; word-wrap:break-word;}
</style>
