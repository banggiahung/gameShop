main_frontEnd = new Vue({
    el: '#main_frontEnd',
    data: {
        dataItems: [],
        CateItems: [],
        dataLatest: [],
        getCategoryFirst: [],
        idGame: 0,
        nameGame: "",
        mainImgGame: "",
        linkDownGame: "",
        detailsGameLayout: "",
        desShortGame: "",
        createDateGame: "",
        cateIdGame: "",
        categoryNameGame: "",
        itemsPerPage: 7, 
        currentPage: 1,
        searchKeyword: "",

        maxDisplayedItems: 8,
        maxDisplayedItemsCate: 8,

        imgBanner1: "",
        imgBanner2: "",
        imgBanner3: "",
    },
    computed: {
        displayedItems() {
            return this.dataItems.slice(0, this.maxDisplayedItems);
        },
        displayedItemsCate() {
            return this.getCategoryFirst.slice(0, this.maxDisplayedItemsCate);
        },
    },
    mounted() {
        axios.get("/Products/GetAllMainView")
            .then((response) => {
                this.imgMainView = response.data[0].imgMain;
                this.imgBanner1 = response.data[0].banner1;
                this.imgBanner2 = response.data[0].banner2;
                this.imgBanner3 = response.data[0].banner3;
                return Promise.resolve();
            });
        axios.get("/Home/GetAllProductLayOut")
            .then((response) => {
                this.dataItems = response.data;
                return Promise.resolve();
            });
        axios.get("/Home/GetTop5LatestProducts")
            .then((response) => {
                this.dataLatest = response.data;
                return Promise.resolve();
            });
        axios.get("/Home/GetCategoryFirst")
            .then((response) => {
                this.getCategoryFirst = response.data;
                return Promise.resolve();
            });

        axios.get("/Home/GetProductLast")
            .then((response) => {
                this.idGame = response.data.id;
                this.nameGame = response.data.name;
                this.mainImgGame = response.data.mainImg;
                this.linkDownGame = response.data.linkDown;
                this.detailsGameLayout = response.data.detailsGame;
                this.desShortGame = response.data.desShort;
                this.createDateGame = response.data.createDate;
                this.cateIdGame = response.data.cateId;
                this.categoryNameGame = response.data.categoryName;
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
            
           
        },
    }
});