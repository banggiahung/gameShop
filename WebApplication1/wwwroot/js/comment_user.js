comment_user = new Vue({
    el: '#comment_user',
    data: {
        dataComment: [],
        commentsPerPage: 5,
        currentPage: 1,
        postText: false,
        CommentByAdmin: "",
        sub: {},
        subIndex : ""
    },
    computed: {
        totalPages() {
            return Math.ceil(this.dataComment.length / this.commentsPerPage);
        },
        visiblePageNumbers() {
            const range = 2;
            const start = Math.max(1, this.currentPage - range);
            const end = Math.min(this.totalPages, this.currentPage + range);
            const pages = [];

            if (start > 1) {
                pages.push(1);
                if (start > 2) {
                    pages.push('...');
                }
            }

            for (let i = start; i <= end; i++) {
                pages.push(i);
            }

            if (end < this.totalPages) {
                if (end < this.totalPages - 1) {
                    pages.push('...');
                }
                pages.push(this.totalPages);
            }

            return pages;
        },
        paginatedComments() {
            const startIndex = (this.currentPage - 1) * this.commentsPerPage;
            const endIndex = startIndex + this.commentsPerPage;
            return this.dataComment.slice(startIndex, endIndex);
        },
    },
    mounted() {
        $('#preloader').fadeIn();

        axios.get(`/Products/GetAllCommentUser`)
            .then((response) => {
                this.dataComment = response.data;
                $('#preloader').fadeOut();
            })
            .catch((error) => {
                console.error(error);
                $('#preloader').fadeOut();
            });
    },
    methods: {
        formatDate(date) {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return date.toLocaleDateString('vi-VN', options);
        },
        changePage(pageNumber) {
            this.currentPage = pageNumber;
        },
        async postAdmin() {
            try {
                const formData = new FormData();
                formData.append('id', this.sub.id);
                formData.append('CommentByAdmin', this.CommentByAdmin);
                await axios.post('/Products/PostCommentAdmin', formData,
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
        close() {
            this.postText = false;
            this.CommentByAdmin = "";
            this.sub = {};
        },
        openComment(sub, subindex) {
            this.postText = true;
            this.subIndex = subindex;
            this.sub = sub;
            console.log(this.sub);
        }
    }
});