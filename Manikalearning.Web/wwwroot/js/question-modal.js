window.initQuestionModal = function () {
    const optionLabels = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];
     
    const pyqSwitch = document.getElementById('pyqSwitch');
    const pyqFields = document.getElementById('pyqFields');
    if (pyqSwitch && pyqFields) {
        pyqSwitch.addEventListener('change', function () {
            console.log("Checked");
            if (this.checked) {
                pyqFields.classList.remove('d-none');
            } else {
                pyqFields.classList.add('d-none');
            }
        });
    }

    // Helper: Get Selected Question Type
    function getSelectedQuestionType() {
        const select = document.getElementById('ddlQuestionType');
        if (!select) return "MCQ";

        const selectedOption = select.options[select.selectedIndex];
        const selectedText = selectedOption ? selectedOption.text.toUpperCase().replace(/\s+/g, '') : '';
        const selectedVal = select.value;

        if (selectedText.includes("MULTIPLE") || selectedText.includes("MSQ") || selectedVal === "2") {
            return "MSQ";
        } else if (selectedText.includes("NUMERICAL") || selectedText.includes("NAT") || selectedText.includes("SUBJECTIVE") || selectedVal === "3" || selectedVal === "5") {
            return "NUMERICAL";
        }
        return "MCQ";
    }

    // 2. Re-index Options
    function reindexOptions() {
        const selectedType = getSelectedQuestionType();
        const rows = document.querySelectorAll('#optionsContainer .option-row');

        rows.forEach((row, index) => {
            const labelText = optionLabels[index] || `Opt ${index + 1}`;
            row.setAttribute('data-index', index);

            const labelTextEl = row.querySelector('.option-label-text');
            if (labelTextEl) labelTextEl.textContent = labelText;

            const idEl = row.querySelector('.option-id');
            if (idEl) idEl.setAttribute('name', `Options[${index}].OptionId`);

            const labelEl = row.querySelector('.option-label');
            if (labelEl) {
                labelEl.setAttribute('name', `Options[${index}].OptionLabel`);
                labelEl.value = labelText;
            }

            const orderEl = row.querySelector('.option-display-order');
            if (orderEl) {
                orderEl.setAttribute('name', `Options[${index}].DisplayOrder`);
                orderEl.value = index + 1;
            }

            const textEnEl = row.querySelector('.option-text-input');
            if (textEnEl) textEnEl.setAttribute('name', `Options[${index}].OptionText`);

            const textHiEl = row.querySelector('.option-text-hi');
            if (textHiEl) textHiEl.setAttribute('name', `Options[${index}].OptionTextHindi`);

            const selectorDiv = row.querySelector('.option-selector-container');
            if (selectorDiv) {
                const existingInput = selectorDiv.querySelector('input');
                const isChecked = existingInput && existingInput.checked ? 'checked' : '';

                if (selectedType === "MSQ") {
                    selectorDiv.innerHTML = `
                        <input class="form-check-input mt-0 option-correct-input" type="checkbox" name="Options[${index}].IsCorrect" value="true" ${isChecked} />
                        <span class="ms-1 small text-muted fs-7">Correct</span>
                    `;
                } else {
                    selectorDiv.innerHTML = `
                        <input class="form-check-input mt-0 option-correct-input" type="radio" name="CorrectOptionIndex" value="${index}" ${isChecked} required />
                        <span class="ms-1 small text-muted fs-7">Correct</span>
                    `;
                }
            }
        });

        // Toggle Remove Button State
        const removeBtns = document.querySelectorAll('.btn-remove-option');
        removeBtns.forEach(btn => {
            btn.disabled = (rows.length <= 2);
        });
    }

    // 3. Question Type Change Handler
    function handleQuestionTypeChange() {
        const type = getSelectedQuestionType();
        const optionsWrapper = document.getElementById('optionsWrapper');
        const optionInputs = document.querySelectorAll('#optionsContainer .option-text-input');
        const hintEl = document.getElementById('optionTypeHint');

        if (type === "NUMERICAL") {
            if (optionsWrapper) optionsWrapper.classList.add('d-none');
            optionInputs.forEach(input => input.required = false);
        } else {
            if (optionsWrapper) optionsWrapper.classList.remove('d-none');
            optionInputs.forEach(input => input.required = true);

            if (hintEl) {
                if (type === "MSQ") {
                    hintEl.textContent = "Select one or more correct options";
                } else {
                    hintEl.textContent = "Select radio button for single correct option";
                }
            }
            reindexOptions();
        }
    }

    // 4. Add Option Row
    function addOptionRow() {
        const container = document.getElementById('optionsContainer');
        if (!container) return;

        const rows = container.querySelectorAll('.option-row');
        const index = rows.length;
        if (index >= 10) return;

        const labelText = optionLabels[index] || `Opt ${index + 1}`;
        const html = `
            <div class="card mb-2 border shadow-none option-row" data-index="${index}">
                <div class="card-body p-2">
                    <div class="input-group input-group-sm mb-1">
                        <span class="input-group-text bg-light fw-bold text-dark px-3 option-label-text">${labelText}</span>
                        <input type="hidden" class="option-id" name="Options[${index}].OptionId" value="00000000-0000-0000-0000-000000000000" />
                        <input type="hidden" class="option-label" name="Options[${index}].OptionLabel" value="${labelText}" />
                        <input type="hidden" class="option-display-order" name="Options[${index}].DisplayOrder" value="${index + 1}" />

                        <input type="text" name="Options[${index}].OptionText" value="" class="form-control shadow-none option-text-input option-text-en" placeholder="Option Text (English)" required />

                        <div class="input-group-text bg-light option-selector-container">
                            <input class="form-check-input mt-0 option-correct-input" type="radio" name="CorrectOptionIndex" value="${index}" required />
                            <span class="ms-1 small text-muted fs-7">Correct</span>
                        </div>
                        <button type="button" class="btn btn-outline-danger btn-remove-option px-2" title="Remove Option">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                    <div class="input-group input-group-sm">
                        <span class="input-group-text bg-light text-muted px-2 fs-7">हिंदी</span>
                        <input type="text" name="Options[${index}].OptionTextHindi" value="" class="form-control shadow-none option-text-hi" placeholder="विकल्प पाठ (हिंदी)" />
                    </div>
                </div>
            </div>`;

        container.insertAdjacentHTML('beforeend', html);
        reindexOptions();
    }

    // Initial Options Setup
    const container = document.getElementById('optionsContainer');
    if (container) {
        if (container.querySelectorAll('.option-row').length === 0) {
            for (let i = 0; i < 4; i++) {
                addOptionRow();
            }
        } else {
            reindexOptions();
        }
    }

    // Add Option Button Listener
    const btnAddOption = document.getElementById('btnAddOption');
    if (btnAddOption) {
        btnAddOption.addEventListener('click', function (e) {
            e.preventDefault();
            addOptionRow();
        });
    }

    // Remove Option Event Delegation
    if (container) {
        container.addEventListener('click', function (e) {
            const btn = e.target.closest('.btn-remove-option');
            if (btn) {
                e.preventDefault();
                const rows = container.querySelectorAll('.option-row');
                if (rows.length > 2) {
                    const row = btn.closest('.option-row');
                    if (row) {
                        row.remove();
                        reindexOptions();
                    }
                }
            }
        });
    }

    // Helper: Fetch JSON & Populate Dropdown
    function populateDropdown(selectElement, url, defaultText) {
        if (!selectElement) return;
        selectElement.innerHTML = `<option value="">${defaultText}</option>`;

        fetch(url)
            .then(res => res.json())
            .then(data => {
                data.forEach(item => {
                    const opt = document.createElement('option');
                    opt.value = item.id || item.value;
                    opt.textContent = item.name || item.text;
                    selectElement.appendChild(opt);
                });
            })
            .catch(err => console.error("Error loading dropdown data:", err));
    }

    // 5. Cascading Dropdowns
    const ddlSubject = document.getElementById('ddlSubject');
    if (ddlSubject) {
        ddlSubject.addEventListener('change', function () {
            const subjectId = this.value;
            const ddlSection = document.getElementById('ddlSection');
            const ddlTopic = document.getElementById('ddlTopic');
            const ddlSubTopic = document.getElementById('ddlSubTopic');

            if (ddlSection) ddlSection.innerHTML = '<option value="">-- Select Section --</option>';
            if (ddlTopic) ddlTopic.innerHTML = '<option value="">-- Select Topic --</option>';
            if (ddlSubTopic) ddlSubTopic.innerHTML = '<option value="">-- Select SubTopic --</option>';

            if (subjectId) {
                populateDropdown(ddlSection, '/Admin/Questions/GetSections?subjectId=' + encodeURIComponent(subjectId), '-- Select Section --');
            }
        });
    }

    const ddlSection = document.getElementById('ddlSection');
    if (ddlSection) {
        ddlSection.addEventListener('change', function () {
            const sectionId = this.value;
            const ddlTopic = document.getElementById('ddlTopic');
            const ddlSubTopic = document.getElementById('ddlSubTopic');

            if (ddlTopic) ddlTopic.innerHTML = '<option value="">-- Select Topic --</option>';
            if (ddlSubTopic) ddlSubTopic.innerHTML = '<option value="">-- Select SubTopic --</option>';

            if (sectionId) {
                populateDropdown(ddlTopic, '/Admin/Questions/GetTopics?sectionId=' + encodeURIComponent(sectionId), '-- Select Topic --');
            }
        });
    }

    const ddlTopic = document.getElementById('ddlTopic');
    if (ddlTopic) {
        ddlTopic.addEventListener('change', function () {
            const topicId = this.value;
            const ddlSubTopic = document.getElementById('ddlSubTopic');
            const ddlPassage = document.getElementById('ddlPassage');

            if (ddlSubTopic) ddlSubTopic.innerHTML = '<option value="">-- Select SubTopic --</option>';
            if (ddlPassage) ddlPassage.innerHTML = '<option value="">-- None --</option>';

            if (topicId) {
                populateDropdown(ddlSubTopic, '/Admin/Questions/GetSubTopics?topicId=' + encodeURIComponent(topicId), '-- Select SubTopic --');
                populateDropdown(ddlPassage, '/Admin/Questions/GetPassages?topicId=' + encodeURIComponent(topicId), '-- None --');
            }
        });
    }

    // Question Type Change Listener
    const ddlQuestionType = document.getElementById('ddlQuestionType');
    if (ddlQuestionType) {
        ddlQuestionType.addEventListener('change', handleQuestionTypeChange);
    }

    // 6. Auto Translate
    const btnAutoTranslate = document.getElementById('btnAutoTranslate');
    if (btnAutoTranslate) {
        btnAutoTranslate.addEventListener('click', function () {
            const txtQuestionEn = document.getElementById('txtQuestionEn');
            if (!txtQuestionEn) return;
            const enText = txtQuestionEn.value;
            if (!enText) return;

            const formData = new URLSearchParams();
            formData.append('text', enText);
            formData.append('targetLang', 'hi');

            fetch('/Admin/Questions/AutoTranslate', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: formData.toString()
            })
            .then(res => res.json())
            .then(res => {
                if (res && res.translatedText) {
                    const txtQuestionHi = document.getElementById('txtQuestionHi');
                    if (txtQuestionHi) txtQuestionHi.value = res.translatedText;
                }
            })
            .catch(err => console.error("Translation error:", err));
        });
    }

    // Initial Trigger
    handleQuestionTypeChange();
};