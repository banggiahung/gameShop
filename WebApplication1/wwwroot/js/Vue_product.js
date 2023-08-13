class MyUploadAdapter {
    constructor(loader, editor) {
        this.loader = loader;
        this.temporaryImageUrl = null;
        this.editor = editor;
    }

    upload() {
        return this.loader.file.then(file => new Promise((resolve, reject) => {
            this._initRequest();
            this._initListeners(resolve, reject, file);
            this._sendRequest(file, resolve, reject);
        }));
    }

    abort() {
        if (this.xhr) {
            this.xhr.abort();
        }
    }

    _initRequest() {
        this.xhr = new XMLHttpRequest();
        this.xhr.open('POST', '/Products/UploadLocalMain', true);
        this.xhr.responseType = 'json';
    }

    _initListeners(resolve, reject, file) {
        const { xhr, loader } = this;
        const genericErrorText = `Couldn't upload file: ${file.name}.`;

        xhr.addEventListener('error', () => reject(genericErrorText));
        xhr.addEventListener('abort', () => reject());
        xhr.addEventListener('load', () => {
            const response = xhr.response;

            if (!response || response.error) {
                return reject(response && response.error ? response.error.message : genericErrorText);
            }

            resolve({
                default: response.url
            });
        });

        if (xhr.upload) {
            xhr.upload.addEventListener('progress', evt => {
                if (evt.lengthComputable) {
                    loader.uploadTotal = evt.total;
                    loader.uploaded = evt.loaded;
                }
            });
        }
    }

    _sendRequest(file) {
        return new Promise((resolve, reject) => {

            const data = new FormData();
            data.append('upload', file)
            const self = this;

            this.xhr.addEventListener('load', function () {
                const response = self.xhr.response;
                if (!response || response.error) {
                    return reject(response && response.error ? response.error.message : genericErrorText);
                }

                const imageUrl = response.urls[0];

                if (!self.editor) {
                    console.error("Editor instance not available.");
                    return;
                }

                const selection = self.editor.model.document.selection;

                const imageElement = document.createElement('img');
                imageElement.src = imageUrl;
                imageElement.alt = "Inserted Image";



                self.editor.setData(`${self.editor.getData()}<img src="${imageUrl}" alt="Inserted Image">`);
                resolve({
                    default: imageUrl
                });
            });

            this.xhr.addEventListener('error', function () {
                console.error("Error during upload.");
            });

            this.xhr.addEventListener('abort', function () {
                console.error("Upload aborted.");
            });

            this.xhr.send(data);
        });
    }
}

function MyCustomUploadAdapterPlugin(editor) {
    editor.plugins.get('FileRepository').createUploadAdapter = loader => new MyUploadAdapter(loader, editor);
}



Admin_vue = new Vue({
    el: '#Admin_vue',
    data: {
        id: 0,
        ckName: "",
        editor: null,
        categoryID: 0,
        CategoryID: 0,
        categoryName: "",
        Part: "",
        CateItems: [],
        PrPath: null,
        imageFile: null,
        previewImage: null,
        uploadedImage: null,
        CPU: "Windows",
        RAM: ["GB", "MB"],
        selectedRAM: "GB",
        selectedGB: "GB"
    },
    mounted() {
        this.getItemsById();

        ClassicEditor
            .create(document.querySelector('#editor'), {
                extraPlugins: [MyCustomUploadAdapterPlugin],
            })

            .then(editor => {
                this.editor = editor;
                if (this.editor && this.editor.model) {
                    this.editor.model.document.on('change:data', () => {
                        console.log(" this.ckName", this.ckName)
                        this.ckName = this.editor.getData();
                    });
                }
            })
            .catch(error => {
                console.error(error);
            });


        axios.get("/Products/GetAllCategory")
            .then((response) => {
                this.CateItems = response.data;
                return Promise.resolve();
            })
    },
    methods: {

        getItemsById() {
            this.id = $("#valueId").val();
            axios.get(`/Products/getIdProducts?id=${this.id}`)
                .then((response) => {
                    this.ckName = response.data.detailsGame;
                    this.CategoryID = response.data.cateId;
                    if (this.editor) {
                        this.editor.setData(this.ckName);
                    }
                    return Promise.resolve();
                });
        },
        onFileChange(event) {
            this.imageFile = event.target.files[0];
            this.previewImage = URL.createObjectURL(this.imageFile);
            this.uploadedImage = null;
        },
        async editProducts() {
            try {

                this.id = $("#valueId").val();
                const formData = new FormData();
                formData.append('Name', $("#name").val());
                formData.append('RAM', $("#ram").val() + this.selectedGB);
                formData.append('GB', $("#GB").val() + this.selectedRAM);
                formData.append('Language', $("#Language").val());
                formData.append('CPU', this.CPU);
                formData.append('LinkDown', $("#downLink").val());
                formData.append('LinkDownDrop', $("#LinkDownDrop").val());
                formData.append('LinkDownMedia', $("#LinkDownMedia").val());
                formData.append('DetailsGame', this.ckName);
                formData.append('DesShort', $("#desShort").val());
                formData.append('Part', $("#Part").val());
                formData.append('CateId', this.CategoryID);
                formData.append('id', this.id);
                if (this.$refs.PrPath.files[0] != null) {
                    formData.append('PrPath', this.$refs.PrPath.files[0]);

                }

                await axios.post(`/Products/EditProduct/id=${this.id}`, formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Đã lưu thành công',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            } catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã có lỗi xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();

                    }
                });
            }
        }
    }
});