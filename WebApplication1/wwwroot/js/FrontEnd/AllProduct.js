all_product = new Vue({
    el: '#all_product',
    data: {
        dataItems: [],
        CateItems: [],
        CategoryID: 0,
        itemsPerPage: 20,
        maxPages: 5,
        currentPage: 1
    },
    computed: {
        filteredDataItems: function () {
            if (this.CategoryID === 0) {
                return this.dataItems;
            } else {
                return this.dataItems.filter(item => item.cateId === this.CategoryID);
            }
        },
        totalItems() {
            return this.filteredDataItems.length;
        },
        totalPages() {
            return Math.ceil(this.totalItems / this.itemsPerPage);
        },
        visiblePages() {
            const startPage = Math.max(1, this.currentPage - Math.floor(this.maxPages / 2));
            const endPage = Math.min(this.totalPages, startPage + this.maxPages - 1);

            let pages = [];
            for (let i = startPage; i <= endPage; i++) {
                pages.push(i);
            }

            return pages;
        },
        paginatedDataItems() {
            const startIndex = (this.currentPage - 1) * this.itemsPerPage;
            const endIndex = Math.min(startIndex + this.itemsPerPage - 1, this.totalItems - 1);
            return this.filteredDataItems.slice(startIndex, endIndex + 1);
        }
    },
    mounted() {
        axios.get("/Home/GetAllProduct")
            .then((response) => {
                this.dataItems = response.data;
                return Promise.resolve();
            });
        axios.get("/Home/GetAllCategory")
            .then((response) => {
                this.CateItems = response.data;
                return Promise.resolve();
            })
    },
    methods: {
        handleCategoryChange() {
            if (this.CategoryID === 0) {
                window.location.reload();

            }
        },
        init() {
            axios.get("/Home/GetAllProduct")
                .then((response) => {
                    this.dataItems = response.data;
                    return Promise.resolve();
                })
        },
        getRandomNumber() {
            return Math.round(Math.random() * 100); 
        },
        changePage(page) {
            this.currentPage = page;
        }
    }
});