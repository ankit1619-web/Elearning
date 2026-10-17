(function () {
    'use strict';

    /* ---------- Back to top ---------- */
    var backBtn = document.getElementById('backToTop');

    if (backBtn) {
        var ticking = false;

        function toggleBackBtn() {
            backBtn.classList.toggle('show', window.scrollY > 300);
            ticking = false;
        }

        window.addEventListener('scroll', function () {
            if (!ticking) {
                window.requestAnimationFrame(toggleBackBtn);
                ticking = true;
            }
        }, { passive: true });

        backBtn.addEventListener('click', function () {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });

        toggleBackBtn();
    }

    /* ---------- Mobile menu: link dabane par band ho jaye ---------- */
    var navCollapse = document.getElementById('mainNavbar');

    if (navCollapse && window.bootstrap) {
        navCollapse.querySelectorAll('.nav-link').forEach(function (link) {
            link.addEventListener('click', function () {
                if (navCollapse.classList.contains('show')) {
                    bootstrap.Collapse.getOrCreateInstance(navCollapse).hide();
                }
            });
        });
    }

    /* ---------- Scroll-spy: page ke sections ke hisaab se active menu ----------
       Sirf un links par chalta hai jinka href "#section" ya "/#section" ho
       aur jiska section current page par maujood ho. */
    var navLinks = Array.prototype.slice.call(
        document.querySelectorAll('#mainNavbar .nav-link[href*="#"]')
    );

    var spyItems = navLinks.map(function (link) {
        var hash = link.hash;                      // "#exams"
        var samePage = link.pathname.replace(/\/$/, '') === window.location.pathname.replace(/\/$/, '');
        var target = hash && samePage ? document.querySelector(hash) : null;
        return target ? { link: link, target: target } : null;
    }).filter(Boolean);

    if (spyItems.length && 'IntersectionObserver' in window) {
        var allLinks = document.querySelectorAll('#mainNavbar .nav-link');

        function setActive(link) {
            allLinks.forEach(function (l) { l.classList.remove('active'); });
            if (link) link.classList.add('active');
        }

        var visible = {};

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (e) { visible[e.target.id] = e.isIntersecting; });

            // Page me jo sabse upar wala dikh raha section hai wahi active
            var current = spyItems.find(function (item) { return visible[item.target.id]; });
            if (current) setActive(current.link);
        }, {
            // header ke neeche, screen ke upar-beech wale hisse ko "current" maano
            rootMargin: '-90px 0px -55% 0px',
            threshold: 0
        });

        spyItems.forEach(function (item) { observer.observe(item.target); });
    }
})();
