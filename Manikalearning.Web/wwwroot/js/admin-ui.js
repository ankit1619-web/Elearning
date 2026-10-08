const Toast = {
     
    bsModalInstance: null,
      
    showToast: function (type, message) {
        const toastContainer = document.getElementById('toastContainer');
        if (!toastContainer) return;

        let icon = '';
        let bgClass = '';
        let btnCloseClass = 'btn-close-white';

        switch (type) {
            case 'success':
                icon = '<i class="bi bi-check-circle-fill fs-5"></i>';
                bgClass = 'bg-success text-white';
                break;
            case 'error':
                icon = '<i class="bi bi-exclamation-triangle-fill fs-5"></i>';
                bgClass = 'bg-danger text-white';
                break;
            case 'warning':
                icon = '<i class="bi bi-exclamation-circle-fill fs-5"></i>';
                bgClass = 'bg-warning text-dark';
                btnCloseClass = 'btn-close-dark';
                break;
            case 'info':
                icon = '<i class="bi bi-info-circle-fill fs-5"></i>';
                bgClass = 'text-white';
                break;
        }

        const toastId = 'toast-' + Date.now();
        const customStyle = type === 'info' ? 'background-color: #6366f1;' : '';

        const toastHTML = `
            <div id="${toastId}" class="toast custom-toast align-items-center ${bgClass} mb-3 border-0 shadow" role="alert" aria-live="assertive" aria-atomic="true" style="${customStyle}">
                <div class="d-flex">
                    <div class="toast-body d-flex align-items-center gap-2">
                        ${icon}
                        <span class="fw-medium">${message}</span>
                    </div>
                    <button type="button" class="btn-close ${btnCloseClass} me-3 m-auto shadow-none" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
         
        toastContainer.insertAdjacentHTML('beforeend', toastHTML);

        const toastElement = document.getElementById(toastId);
        const bsToast = new bootstrap.Toast(toastElement, { delay: 4000 });  
        bsToast.show();
         
        toastElement.addEventListener('hidden.bs.toast', function () {
            toastElement.remove();
        });
    },
     
    success: function (message) { this.showToast('success', message); },
    error: function (message) { this.showToast('error', message); },
    warning: function (message) { this.showToast('warning', message); },
    info: function (message) { this.showToast('info', message); },
     
    showLoader: function () {
        const loader = document.getElementById('globalLoader');
        if (loader) loader.classList.remove('d-none');
    },

    hideLoader: function () {
        const loader = document.getElementById('globalLoader');
        if (loader) loader.classList.add('d-none');
    },

    
    openModal: function (url, sizeClass = '') {
        Toast.showLoader();

        const modalDialog = document.getElementById('globalModalSize');
        const modalContent = document.getElementById('globalModalContent');

        if (!modalDialog || !modalContent) {
            Toast.hideLoader();
            Toast.error("Global modal container missing in layout.");
            return;
        } 
        modalDialog.className = 'modal-dialog modal-dialog-centered ' + sizeClass;
         
        $.ajax({
           url: url,
           type: 'GET',
           success: function (response) {
               Toast.hideLoader(); 
               let htmlContent = "";
               if (typeof response === 'object' && response.html) {
                   htmlContent = response.html;  
               } else {
                   htmlContent = response;  
               } 
               modalContent.innerHTML = '';
               const range = document.createRange();
               range.selectNode(modalContent);
               const fragment = range.createContextualFragment(htmlContent);
               modalContent.appendChild(fragment);
        
               if (typeof window.initQuestionModal === 'function') {
                   window.initQuestionModal();
               }
        
               const modalEl = document.getElementById('globalModal');
               Toast.bsModalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);
               Toast.bsModalInstance.show();
           },
           error: function (xhr) {
               Toast.hideLoader();
               Toast.error("Failed to load request form. Please try again.");
           }
       });
    },
     
    closeModal: function () {
        if (Toast.bsModalInstance) {
            Toast.bsModalInstance.hide();
        }
    },
     
    submitModalForm: function (formElement, successCallback) {
        Toast.showLoader();
        const $form = $(formElement);
        var formData = new FormData(formElement);
        $.ajax({
            url: $form.attr('action'),
            type: $form.attr('method') || 'POST',
            data: formData,
            contentType: false, // Required for File Upload
            processData: false,
            success: function (response) {
                Toast.hideLoader();
                 
                if (typeof response === 'object' && response !== null) {
                    if (response.isSuccess) {
                        Toast.closeModal();
                        Toast.success(response.message || "Operation completed successfully!");

                        if (typeof successCallback === 'function') {
                            successCallback(response);
                        } else {
                            setTimeout(() => location.reload(), 800);
                        }
                    } else { 
                        const errorMsg = (response.errors && response.errors.length > 0)
                            ? response.errors.join(' | ')
                            : response.message;
                        Toast.error(errorMsg || "Operation failed.");
                    }
                } 
                else {
                    $('#globalModalContent').html(response);
                }
            },
            error: function (xhr) {
                Toast.hideLoader();
                Toast.error("An error occurred while submitting data.");
            }
        });

        return false; 
    }
};