var footerV2 = new Vue({
    el: "#footerV2",
    data: {
        page: []
    },
    computed: {
        domainName() {
            return window.location.hostname;
        },
    },
    mounted() {
      
        axios.get("/User/GetAllTitile")
            .then((response) => {
                this.page = response.data.page;
                return Promise.resolve();
            })
    },

});
