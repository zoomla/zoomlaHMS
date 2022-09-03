<template>
    <div class="page">
        <div class="content">
            <canvas ref="canvas" v-bind:width="canvas.size" v-bind:height="canvas.size"></canvas>
            <div>
                <div class="mb-3">
                    <div class="w-100 border" style="max-width:300px; height:60px;" v-bind:style="{background:'#'+color.hex}"></div>
                </div>
                <div class="mb-3">
                    <label class="form_label">Hex</label>
                    <div class="input-group">
                        <span class="input-group-text">#</span>
                        <input v-model="color.hex" class="form-control" readonly />
                    </div>
                </div>
                <div class="mb-3">
                    <label class="form_label">RGB</label>
                    <div class="input-group">
                        <span class="input-group-text">R</span>
                        <input v-model="color.rgb.r" class="form-control" readonly />
                        <span class="input-group-text">G</span>
                        <input v-model="color.rgb.g" class="form-control" readonly />
                        <span class="input-group-text">B</span>
                        <input v-model="color.rgb.b" class="form-control" readonly />
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
                color: {
                    hex: "",
                    rgb: {
                        r: 0,
                        g: 0,
                        b: 0,
                    }
                },
                canvas: {
                    size: 300,
                    instance: null,
                },
                timer: null,
                taking: false,
            };
        },
        mounted() {
            const that = this;
            cscSetup("TakeColor");

            this.canvas.instance = this.$refs.canvas.getContext("2d");
            this.timer = setInterval(() => {
                csc("GetHexColor").then(res => {
                    that.color.hex = res;
                });
                csc("GetRgbColor").then(res => {
                    let values = res.split(",");
                    that.color.rgb.r = values[0];
                    that.color.rgb.g = values[1];
                    that.color.rgb.b = values[2];
                });

                csc("GetScreenBlockBase64Image").then(res => {
                    let ctx = that.canvas.instance;
                    let img = new Image();
                    img.src = "data:image/jpeg;base64," + res;
                    ctx.drawImage(img, 0, 0, that.canvas.size, that.canvas.size);
                    ctx.beginPath();
                    ctx.lineWidth = 2;
                    ctx.strokeStyle =  "#ff0000";
                    ctx.moveTo(that.canvas.size / 2 - 1, 0);
                    ctx.lineTo(that.canvas.size / 2 - 1, that.canvas.size);
                    ctx.moveTo(0, that.canvas.size / 2 - 1);
                    ctx.lineTo(that.canvas.size, that.canvas.size / 2 - 1);
                    ctx.stroke();
                    ctx.closePath();
                });
            }, 50);

            csc("StartTake");
        },
        beforeUnmount() {
            clearInterval(this.timer);
            csc("StopTake");
        },
        methods: {
            back() {
                app.navigateBack();
            },
        },
    }
</script>

<style>
.content{display:flex;}
.content>canvas{flex-shrink:0; margin-right:.5rem;}
.content>div{width:100%;}
.content>div input,.content>div .input-group{max-width:300px;}
.content>div input:disabled{background:#fff;}
</style>
