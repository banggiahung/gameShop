﻿main_product = new Vue({
    el: '#main_product',
    data: {
        dataItems: [],
        ProductName: "",
        Description: "",
        LinkDown: "",
        LinkDownMedia: "",
        LinkDownDrop: "",
        CategoryID: 0,
        PrPath: null,
        imageFile: null,
        previewImage: null,
        uploadedImage: null,
        productID: 0,
        product_ImagePath: "",
        id: "",
        categoryID: 0,
        categoryName: "",
        CateItems: [],
        itemsPerPage: 7,
        currentPage: 1,
        searchKeyword: "",
    },
    computed: {
        filteredDataItems() {
            
            return this.dataItems.filter((product) =>
                product.name.toLowerCase().includes(this.searchKeyword.toLowerCase())
            );
        },

        
        totalPages() {
            return Math.ceil(this.filteredDataItems.length / this.itemsPerPage);
        },

   
        paginatedDataItems() {
            const startIndex = (this.currentPage - 1) * this.itemsPerPage;
            const endIndex = startIndex + this.itemsPerPage;
            return this.filteredDataItems.slice(startIndex, endIndex);
        },


        visiblePages() {
            
            const totalVisiblePages = 5;
            const halfVisiblePages = Math.floor(totalVisiblePages / 2);
            let startPage = this.currentPage - halfVisiblePages;
            let endPage = this.currentPage + halfVisiblePages;

            if (startPage <= 0) {
                startPage = 1;
                endPage = Math.min(totalVisiblePages, this.totalPages);
            } else if (endPage > this.totalPages) {
                endPage = this.totalPages;
                startPage = Math.max(1, this.totalPages - totalVisiblePages + 1);
            }

            return Array.from({ length: endPage - startPage + 1 }, (_, index) => startPage + index);
        },
    },
    mounted() {
        axios.get("/Products/GetAllProduct")
            .then((response) => {
                this.dataItems = response.data;
                return Promise.resolve();
            });
        this.initData();
    },
    methods: {
        handleSearch() {
            this.currentPage = 1;
        },
        formatDate(date) {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return date.toLocaleDateString('vi-VN', options);
        },
        prevPage() {
            // Điều hướng đến trang trước đó
            if (this.currentPage > 1) {
                this.currentPage -= 1;
            }
        },
        nextPage() {
            // Điều hướng đến trang kế tiếp
            if (this.currentPage < this.totalPages) {
                this.currentPage += 1;
            }
        },
        gotoPage(pageNumber) {
            // Điều hướng đến trang có số thứ tự pageNumber
            this.currentPage = pageNumber;
        },
        initData() {

            axios.get("/Products/GetAllCategory")
                .then((response) => {
                    this.CateItems = response.data;
                    return Promise.resolve();
                })
        },
        onFileChange(event) {
            this.imageFile = event.target.files[0];
            this.previewImage = URL.createObjectURL(this.imageFile);
            this.uploadedImage = null;
        },
        async addProducts() {
            try {
                if (this.LinkDown === "" && this.LinkDownDrop === "" && this.LinkDownMedia === "") {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Không được để trống 1 trong 3 link tải',
                        confirmButtonText: 'OK'
                    });
                    return;
                }
                if (this.Name == "") {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Không được để trống tên game',
                        confirmButtonText: 'OK'
                    });
                    return;
                }
                if (this.$refs.PrPath.files[0] == null) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Không được để trống ảnh game',
                        confirmButtonText: 'OK'
                    });
                    return;
                }
                const formData = new FormData();
                formData.append('Name', this.ProductName);
                formData.append('LinkDownDrop', this.LinkDownDrop);
                formData.append('LinkDownMedia', this.LinkDownMedia);
                formData.append('LinkDown', this.LinkDown);
                formData.append('DesShort', this.Description);
                formData.append('CateId', this.CategoryID);
                formData.append('PrPath', this.$refs.PrPath.files[0]);

                await axios.post('/Products/AddProduct', formData,
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
        getItemsById(id) {
            axios.get(`/Products/getIdProducts?id=${id}`)
                .then((response) => {
                    this.ProductName = response.data.productName;
                    this.Description = response.data.description;
                    this.Slug = response.data.slug;
                    this.Price = response.data.price;
                    this.CategoryID = response.data.categoryID;
                    this.product_ImagePath = response.data.imageMain;
                    this.productID = response.data.productID;
                    this.id = response.data.id;
                    this.quantity = response.data.quantity;
                    return Promise.resolve();
                });
            this.resetDataImg();
        },
        resetData() {
            this.ProductName = null;
            this.Description = null;
            this.CategoryID = 0;
            this.product_ImagePath = null;
            this.productID = null;
            this.id = null;
            this.previewImage = null;
            this.quantity = 0;


        },
        resetDataImg() {
            this.previewImage = null;
            this.uploadedImage = null;
            this.imageFile = null;

        },
        getItemsByIdDelete(id) {
            axios.get(`/Products/getIdProducts/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    if (this.id != null) {
                        Swal.fire({
                            title: 'Xóa sản phẩm',
                            text: 'Bạn có chắc chắn muốn xóa',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Đồng ý',
                            cancelButtonText: 'Không!!!'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                const formData = new FormData();
                                formData.append('id', this.id);
                                axios.post('/Products/deleteProducts', formData, {
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