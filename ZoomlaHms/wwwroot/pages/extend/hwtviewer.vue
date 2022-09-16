<template>
    <div class="page">
        <!-- <div class="page_header">
            <div class="page_header_content">
                <button class="page_header_btn" v-on:click="back"><i class="zi zi_share mirror"></i>返回前一页</button>
                <span class="page_header_title">主题包审计</span>
            </div>
        </div> -->
        <div class="mb-3">
            <label class="form_label">选择.hwt文件</label>
            <div class="input-group">
                <input v-model="filePath" class="form-control bg-white" readonly />
                <button class="btn btn-outline-secondary" v-on:click="chooseHwt"><i class="zi zi_floderOpen"></i></button>
                <button class="btn btn-outline-secondary" v-on:click="openFile"><i class="zi zi_eye"></i></button>
            </div>
        </div>
        <div class="dropane mb-3">
            <i class="zi zi_cube" style="margin-right:.15rem; position:relative; top:1px;"></i>
            <span>拖动文件到此处</span>
        </div>
        <div class="text-center">
            <button class="btn btn-info" v-on:click="openHwt"><i class="zi zi_dna"></i>开始审计</button>
        </div>

        <div style="transform:translateY(50px); color:#343a40;">
            <div>本工具支持查看与更新 .hwt 主题包内文件，以下为使用步骤：</div>
            <div>1、点击“选择.hwt文件”下输入框后的文件夹按钮（输入框后第一个按钮），选择文件；或直接在输入框内输入文件路径；或拖动文件到输入框下方灰色区域。</div>
            <div>2、点击下方“开始审计”按钮后，即可在新弹出的窗口内查看包内文件。</div>
            <div>在新窗口中可以管理包内文件，操作说明：</div>
            <div>- 双击列表项：打开文件或文件夹</div>
            <div>- 拖放文件/文件夹进入窗口：添加/更新文件或文件夹</div>
            <div>- 选中文件然后按下键盘DELETE键：删除文件或文件夹</div>
        </div>
    </div>
</template>

<script>
    export default {
        data() {
            return {
                filePath: "",
                timer: null,
            };
        },
        mounted() {
            const that = this;
            cscSetup("HwtViewer");

            this.timer = setInterval(function() {
                csc("GetFilePath").then(res => {
                    that.filePath = res;
                });
            }, 200);
        },
        beforeUnmount() {
            clearInterval(this.timer);
        },
        methods: {
            chooseHwt() {
                const that = this;
                csc("ChooseFile");
            },
            openHwt() {
                csc("OpenFile");
            },
            openFile() {
                CSharp.openFile(this.filePath);
            },
            back() {
                app.navigateBack();
            },
        },
    }
</script>

<style>
.dropane{display:flex; justify-content:center; align-items:center; height:10rem; color:#adb5bd; font-size:1.375rem; background:#FCFDFD; border:2px dashed #e9ecef; border-radius:.375rem;}
</style>
