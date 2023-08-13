main_view_admin = new Vue({
    el: '#main_view_admin',
    data: {
        dataItems: [],
        dataNoti: [],
        idMainView: 0,
        idNoti: 1,
        imgMainView: "",
        imgBanner1: "",
        imgBanner2: "",
        imgBanner3: "",
        passAdmin:"",

        PrPath: null,
        imageFile: null,
        previewImage: null,
        uploadedImage: null,

        // banner 1
        PrPath1: null,
        imageFile1: null,
        previewImage1: null,
        uploadedImage1: null,
         // banner 2
        PrPath2: null,
        imageFile2: null,
        previewImage2: null,
        uploadedImage2: null,
        // banner 3
        PrPath3: null,
        imageFile3: null,
        previewImage3: null,
        uploadedImage3: null,

        Noti: ""
    },
    mounted() {
        axios.get("/Products/GetAllMainView")
            .then((response) => {
                this.dataItems = response.data;
                this.idMainView = response.data[0].id;
                this.imgMainView = response.data[0].imgMain;
                this.imgBanner1 = response.data[0].banner1;
                this.imgBanner2 = response.data[0].banner2;
                this.imgBanner3 = response.data[0].banner3;
                return Promise.resolve();
            });
        axios.get("/Products/GetAllNoti")
            .then((response) => {
                this.dataNoti = response.data;
                return Promise.resolve();
            });
    },
    methods: {
        formatDate(date) {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return date.toLocaleDateString('vi-VN', options);
        },
        onFileChange(event) {
            this.imageFile = event.target.files[0];
            this.previewImage = URL.createObjectURL(this.imageFile);
            this.uploadedImage = null;
        },
        onFileChange1(event) {
            this.imageFile1 = event.target.files[0];
            this.previewImage1 = URL.createObjectURL(this.imageFile1);
            this.uploadedImage1 = null;
        },
        onFileChange2(event) {
            this.imageFile2 = event.target.files[0];
            this.previewImage2 = URL.createObjectURL(this.imageFile2);
            this.uploadedImage2 = null;
        },
        onFileChange3(event) {
            this.imageFile3 = event.target.files[0];
            this.previewImage3 = URL.createObjectURL(this.imageFile3);
            this.uploadedImage3 = null;
        },
        getItemsById(id) {
            axios.get(`/Products/getIdNoti?id=${id}`)
                .then((response) => {
                    this.idNoti = response.data.id;
                    this.Noti = response.data.noti;
                    return Promise.resolve();
                });
            this.resetDataImg();
        },
        async editMainView() {
            try {
                const formData = new FormData();
                formData.append('id', this.idMainView);

                if (this.$refs.PrPath1.files[0] != null) {
                    formData.append('PrPath', this.$refs.PrPath1.files[0]);
                }
                if (this.$refs.PrPath2.files[0] != null) {
                    formData.append('PrPath1', this.$refs.PrPath2.files[0]);
                }
                if (this.$refs.PrPath3.files[0] != null) {
                    formData.append('PrPath2', this.$refs.PrPath3.files[0]);
                }
                if (this.$refs.PrPath4.files[0] != null) {
                    formData.append('PrPath3', this.$refs.PrPath4.files[0]);
                }
                await axios.post('/Products/UpdateMainView', formData,
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
        },
        async addNoti() {
            try {
                const formData = new FormData();
                formData.append('Noti', this.Noti);

                await axios.post('/Products/AddNoti', formData,
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
        },
        async editNoti() {
            try {
                const formData = new FormData();
                formData.append('Noti', this.Noti);
                formData.append('id', this.idNoti);

                await axios.post('/Products/UpdateNoti', formData,
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
        },
        async editAdmin() {
            try {
                if (!/[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]/.test(this.passAdmin)) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Mật khẩu không hợp lệ. Vui lòng sử dụng ít nhất một kí tự đặc biệt.',
                        confirmButtonText: 'OK'
                    });
                    return;
                }

                const formData = new FormData();
                formData.append('pass', this.passAdmin);

                await axios.post('/Products/ChangePassword', formData,
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
        },
        getItemsByIdDelete(id) {
            axios.get(`/Products/getIdNoti/${id}`)
                .then((response) => {
                    this.idNoti = response.data.id;
                    if (this.idNoti != null) {
                        Swal.fire({
                            title: 'Xóa thông báo',
                            text: 'Bạn có chắc chắn muốn xóa',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Đồng ý',
                            cancelButtonText: 'Không!!!'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                const formData = new FormData();
                                formData.append('id', this.idNoti);
                                axios.post('/Products/DeleteNoti', formData, {
                                    headers: {
                                        'Content-Type': 'application/x-www-form-urlencoded'
                                    }
                                }).then(response => {
                                    Swal.fire({
                                        icon: 'success',
                                        title: 'Thành công',
                                        text: 'Đã thành công',
                                        confirmButtonText: 'OK',
                                    }).then((result) => {
                                        if (result.isConfirmed) {
                                            window.location.reload();
                                        }
                                    });

                                }).catch(error => {
                                    Swal.fire({
                                        icon: 'error',
                                        title: 'Lỗi',
                                        text: 'Đã có lỗi xảy ra vui lòng thử lại',
                                        confirmButtonText: 'OK'
                                    });
                                });
                            } else {
                                return;
                            }
                        });
                    }
                }).catch((error) => {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Đã có lỗi xảy ra vui lòng thử lại',
                        confirmButtonText: 'OK'
                    });
                })
        },
    }
});