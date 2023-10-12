// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
if (axios) {
    axios.interceptors.request.use(
        config => {
            const token = localStorage.getItem('token');
            if (token) {
                config.headers['Authorization'] = 'Bearer ' + token;
            }
            return config;
        },
        error => {
            return Promise.reject(error);
        }
    );

    axios.interceptors.response.use(
        response => {
            return response;
        },
        error => {
            if (error.response.status === 401) {
                localStorage.removeItem('token');
                window.location.href = '/'; 
            }
            return Promise.reject(error);
        }
    );
} else {
    console.error('Axios is not available!');
}