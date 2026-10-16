

const TeacherAjax = {
    token: function () {
        return $('input[name="__RequestVerificationToken"]').first().val();
    },
     
    post: function (url, data, done) {
        data = data || {};
        data.__RequestVerificationToken = TeacherAjax.token();
        Toast.showLoader();
        $.post(url, data)
            .done(function (r) {
                Toast.hideLoader();
                if (r && r.isSuccess) {
                    Toast.success(r.message || 'Done');
                    Toast.closeModal();
                    if (typeof done === 'function') done(r);
                    else setTimeout(function () { location.reload(); }, 600);
                } else {
                    Toast.error((r && r.message) || 'Operation failed.');
                }
            })
            .fail(function () {
                Toast.hideLoader();
                Toast.error('Request failed. Please try again.');
            });
    },
     
    confirmPost: function (title, message, url, data, done) {
        TeacherUI.confirm(title, message, function () { TeacherAjax.post(url, data, done); });
    }
};

const TeacherUI = {
    _confirmCb: null,

    confirm: function (title, message, onYes) {
        document.getElementById('confirmTitle').textContent = title;
        document.getElementById('confirmMessage').textContent = message;
        TeacherUI._confirmCb = onYes;
        bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmModal')).show();
    },
     
    filterTable: function (inputId, tableId, cellIndex) {
        const f = document.getElementById(inputId).value.toLowerCase();
        const rows = document.getElementById(tableId).getElementsByTagName('tr');
        for (let i = 1; i < rows.length; i++) {
            const td = rows[i].getElementsByTagName('td')[cellIndex || 0];
            if (td) rows[i].style.display = td.textContent.toLowerCase().indexOf(f) > -1 ? '' : 'none';
        }
    }
};

document.addEventListener('DOMContentLoaded', function () {
    const btn = document.getElementById('confirmYesBtn');
    if (btn) {
        btn.addEventListener('click', function () {
            bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmModal')).hide();
            if (typeof TeacherUI._confirmCb === 'function') TeacherUI._confirmCb();
            TeacherUI._confirmCb = null;
        });
    }
});
 
const TeacherLookup = {
    reset: function (sel, placeholder) {
        const el = document.querySelector(sel);
        if (el) el.innerHTML = '<option value="">' + (placeholder || '-- Select --') + '</option>';
    },

    fill: function (sel, kind, parentId, placeholder, selectedId) {
        const el = document.querySelector(sel);
        if (!el) return Promise.resolve();
        TeacherLookup.reset(sel, placeholder);
        if (!parentId && kind !== 'Exams' && kind !== 'Subjects' && kind !== 'Passages') return Promise.resolve();

        const url = '/Teacher/Lookup/' + kind + (parentId ? '?parentId=' + encodeURIComponent(parentId) : '');
        return fetch(url)
            .then(function (r) { return r.json(); })
            .then(function (items) {
                items.forEach(function (it) {
                    const o = document.createElement('option');
                    o.value = it.id;
                    o.textContent = it.name;
                    if (selectedId && String(selectedId).toLowerCase() === String(it.id).toLowerCase()) o.selected = true;
                    el.appendChild(o);
                });
            })
            .catch(function (e) { console.error('Lookup failed', e); });
    },
     
    link: function (parentSel, childSel, kind, placeholder, resetSels) {
        const p = document.querySelector(parentSel);
        if (!p) return;
        p.addEventListener('change', function () {
            (resetSels || []).forEach(function (s) { TeacherLookup.reset(s.sel, s.placeholder); });
            TeacherLookup.fill(childSel, kind, p.value, placeholder);
        });
    }
};
