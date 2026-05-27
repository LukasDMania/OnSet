/**
 * site.js — Management System UI Layer
 * Handles: scroll effects · ripple buttons · scroll-reveal · auto-toast
 *          table column sorting · skeleton loading · active nav · keyboard shortcuts
 */

(function () {
    'use strict';

    /* ──────────────────────────────────────────────────────────
       1. Sticky Navbar Shadow on Scroll
    ────────────────────────────────────────────────────────── */
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        const onScroll = () => navbar.classList.toggle('scrolled', window.scrollY > 12);
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    /* ──────────────────────────────────────────────────────────
       2. Button Ripple Effect
       Attach to any .btn automatically
    ────────────────────────────────────────────────────────── */
    function attachRipple(btn) {
        btn.addEventListener('click', function (e) {
            const r = document.createElement('span');
            r.classList.add('ripple');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height) * 1.5;
            r.style.cssText = `width:${size}px;height:${size}px;left:${e.clientX - rect.left - size / 2}px;top:${e.clientY - rect.top - size / 2}px`;
            this.appendChild(r);
            r.addEventListener('animationend', () => r.remove());
        });
    }

    document.querySelectorAll('.btn').forEach(attachRipple);

    // Watch for dynamically added buttons
    const btnObserver = new MutationObserver(mutations => {
        mutations.forEach(m => m.addedNodes.forEach(node => {
            if (node.nodeType !== 1) return;
            if (node.matches('.btn')) attachRipple(node);
            node.querySelectorAll?.('.btn').forEach(attachRipple);
        }));
    });
    btnObserver.observe(document.body, { childList: true, subtree: true });

    /* ──────────────────────────────────────────────────────────
       3. Scroll-Reveal (Intersection Observer)
       Add class .fade-in to any element; it appears on scroll
    ────────────────────────────────────────────────────────── */
    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                revealObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

    document.querySelectorAll('.fade-in').forEach(el => revealObserver.observe(el));

    // Auto-assign fade-in to key structural elements if not already present
    const autoReveal = ['.card', '.stat-card', '.table-responsive', '.alert'];
    autoReveal.forEach(sel => {
        document.querySelectorAll(sel).forEach((el, i) => {
            if (!el.classList.contains('fade-in') && !el.closest('.no-animate')) {
                el.classList.add('fade-in');
                if (i < 4) el.classList.add(`delay-${i + 1}`);
                revealObserver.observe(el);
            }
        });
    });

    /* ──────────────────────────────────────────────────────────
       4. Table Column Sorting (client-side)
       Add data-sort to <th> elements to enable
       Example: <th data-sort="text">Name</th>
                <th data-sort="number">Amount</th>
                <th data-sort="date">Date</th>
    ────────────────────────────────────────────────────────── */
    document.querySelectorAll('th[data-sort]').forEach(th => {
        th.classList.add('sortable');
        th.setAttribute('title', 'Click to sort');
        th.addEventListener('click', function () {
            const table = this.closest('table');
            const tbody = table.tBodies[0];
            if (!tbody) return;

            const col = Array.from(this.parentElement.children).indexOf(this);
            const type = this.dataset.sort;
            const asc = this.dataset.sortDir !== 'asc';
            this.dataset.sortDir = asc ? 'asc' : 'desc';

            // Reset sibling headers
            this.parentElement.querySelectorAll('th[data-sort]').forEach(h => {
                delete h.dataset.sortDir;
                h.classList.remove('sort-asc', 'sort-desc');
            });
            this.classList.add(asc ? 'sort-asc' : 'sort-desc');
            this.dataset.sortDir = asc ? 'asc' : 'desc';

            const rows = Array.from(tbody.rows);
            rows.sort((a, b) => {
                const av = a.cells[col]?.textContent.trim() ?? '';
                const bv = b.cells[col]?.textContent.trim() ?? '';

                if (type === 'number') return (parseFloat(av) - parseFloat(bv)) * (asc ? 1 : -1);
                if (type === 'date') return (new Date(av) - new Date(bv)) * (asc ? 1 : -1);
                return av.localeCompare(bv, undefined, { sensitivity: 'base' }) * (asc ? 1 : -1);
            });

            // Animate out → reorder → animate in
            tbody.style.opacity = '0';
            tbody.style.transition = 'opacity .15s ease';
            setTimeout(() => {
                rows.forEach(r => tbody.appendChild(r));
                tbody.style.opacity = '1';
            }, 150);
        });
    });

    /* ──────────────────────────────────────────────────────────
       5. Skeleton Loading for Tables
       Call showTableSkeleton(table, rowCount) before fetch,
       then call hideTableSkeleton(table) when data is ready.
    ────────────────────────────────────────────────────────── */
    window.showTableSkeleton = function (table, rows = 5) {
        if (!table) return;
        const tbody = table.tBodies[0];
        const colCount = table.tHead?.rows[0]?.cells.length ?? 4;
        if (!tbody) return;
        tbody.dataset.original = tbody.innerHTML;
        tbody.innerHTML = '';
        for (let i = 0; i < rows; i++) {
            const tr = document.createElement('tr');
            tr.classList.add('table-skeleton');
            for (let j = 0; j < colCount; j++) {
                const sizes = ['sm', 'md', 'lg'];
                const td = document.createElement('td');
                td.innerHTML = `<span class="skeleton ${sizes[(i + j) % 3]}"></span>`;
                tr.appendChild(td);
            }
            tbody.appendChild(tr);
        }
    };

    window.hideTableSkeleton = function (table) {
        if (!table) return;
        const tbody = table.tBodies[0];
        if (tbody?.dataset.original !== undefined) {
            tbody.innerHTML = tbody.dataset.original;
            delete tbody.dataset.original;
        }
    };

    /* ──────────────────────────────────────────────────────────
       6. Toast Notification System
       Usage: showToast('Saved successfully', 'success')
       Types: success | danger | warning | info (default: info)
    ────────────────────────────────────────────────────────── */
    const ICONS = {
        success: '✓',
        danger: '✕',
        warning: '⚠',
        info: 'ℹ',
    };

    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.setAttribute('aria-live', 'polite');
        Object.assign(toastContainer.style, {
            position: 'fixed',
            bottom: '1.25rem',
            right: '1.25rem',
            zIndex: '9999',
            display: 'flex',
            flexDirection: 'column',
            gap: '.5rem',
            maxWidth: '340px',
            width: '100%',
            pointerEvents: 'none',
        });
        document.body.appendChild(toastContainer);
    }

    window.showToast = function (message, type = 'info', duration = 4000) {
        const toast = document.createElement('div');
        toast.setAttribute('role', 'alert');
        toast.innerHTML = `
      <div style="display:flex;align-items:center;gap:.625rem;padding:.75rem 1rem;
                  background:var(--bg-elevated);border:1px solid var(--border-strong);
                  border-radius:var(--radius-lg);box-shadow:var(--shadow-md);
                  font-size:.875rem;color:var(--text-primary);
                  pointer-events:all;cursor:pointer;
                  border-left:3px solid var(--${type === 'info' ? 'info' : type});">
        <span style="font-weight:700;color:var(--${type === 'info' ? 'info' : type});flex-shrink:0">${ICONS[type] ?? ICONS.info}</span>
        <span style="flex:1;color:var(--text-primary)">${message}</span>
        <span style="color:var(--text-muted);font-size:1.1rem;line-height:1" aria-label="dismiss">&times;</span>
      </div>
    `;

        const inner = toast.firstElementChild;
        inner.addEventListener('click', () => dismiss());

        Object.assign(toast.style, {
            opacity: '0',
            transform: 'translateX(20px)',
            transition: 'opacity .25s ease, transform .25s ease',
        });

        toastContainer.appendChild(toast);
        requestAnimationFrame(() => {
            toast.style.opacity = '1';
            toast.style.transform = 'translateX(0)';
        });

        let timer = setTimeout(dismiss, duration);
        inner.addEventListener('mouseenter', () => clearTimeout(timer));
        inner.addEventListener('mouseleave', () => { timer = setTimeout(dismiss, 1500); });

        function dismiss() {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(20px)';
            setTimeout(() => toast.remove(), 300);
        }
    };

    /* ──────────────────────────────────────────────────────────
       7. Active Nav Link Highlighting (based on current path)
    ────────────────────────────────────────────────────────── */
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar .nav-link, .sidebar-item').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (!href || href === '/') return;
        if (currentPath.startsWith(href)) link.classList.add('active');
    });

    /* ──────────────────────────────────────────────────────────
       8. Smooth Count-Up Animation for Stat Cards
       Triggers when stat value enters viewport
    ────────────────────────────────────────────────────────── */
    function easeOutQuart(t) { return 1 - Math.pow(1 - t, 4); }

    function countUp(el) {
        const raw = el.dataset.countTarget ?? el.textContent;
        const prefix = (raw.match(/^[^0-9\-]*/)?.[0]) ?? '';
        const suffix = (raw.match(/[^0-9\.]+$/)?.[0]) ?? '';
        const target = parseFloat(raw.replace(/[^0-9.]/g, ''));
        if (isNaN(target)) return;

        const duration = 900;
        const start = performance.now();
        el.dataset.countTarget = raw;

        function frame(now) {
            const progress = Math.min((now - start) / duration, 1);
            const value = easeOutQuart(progress) * target;
            const display = Number.isInteger(target) ? Math.round(value) : value.toFixed(1);
            el.textContent = prefix + display.toLocaleString() + suffix;
            if (progress < 1) requestAnimationFrame(frame);
        }
        requestAnimationFrame(frame);
    }

    const countObserver = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                countUp(entry.target);
                countObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.5 });

    document.querySelectorAll('.stat-value[data-count-target], .stat-value').forEach(el => {
        if (/^[\$€£]?[\d,]+\.?\d*[KMB%]?$/.test(el.textContent.trim())) {
            countObserver.observe(el);
        }
    });

    /* ──────────────────────────────────────────────────────────
       9. Auto-dismiss Bootstrap Alerts after 6 s
    ────────────────────────────────────────────────────────── */
    document.querySelectorAll('.alert.alert-success, .alert.auto-dismiss').forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity .4s ease, max-height .4s ease, margin .4s ease, padding .4s ease';
            alert.style.opacity = '0';
            alert.style.maxHeight = '0';
            alert.style.margin = '0';
            alert.style.padding = '0';
            alert.style.overflow = 'hidden';
            setTimeout(() => alert.remove(), 450);
        }, 6000);
    });

    /* ──────────────────────────────────────────────────────────
       10. Keyboard Shortcuts
       Alt+S  → focus first .form-control (search / filter)
       Alt+N  → click first .btn-primary (new / save)
       Escape → close open dropdown / modal
    ────────────────────────────────────────────────────────── */
    document.addEventListener('keydown', e => {
        if (e.altKey && e.key === 's') {
            e.preventDefault();
            document.querySelector('.form-control')?.focus();
        }
        if (e.altKey && e.key === 'n') {
            e.preventDefault();
            document.querySelector('.btn-primary')?.click();
        }
        if (e.key === 'Escape') {
            // Close any open Bootstrap dropdowns
            document.querySelectorAll('.dropdown-menu.show').forEach(m => {
                m.previousElementSibling?.click();
            });
        }
    });

    /* ──────────────────────────────────────────────────────────
       11. Form Dirty State (unsaved changes warning)
       Add class .watch-dirty to any <form> to enable
    ────────────────────────────────────────────────────────── */
    document.querySelectorAll('form.watch-dirty').forEach(form => {
        let dirty = false;

        form.addEventListener('change', () => { dirty = true; });
        form.addEventListener('submit', () => { dirty = false; });

        window.addEventListener('beforeunload', e => {
            if (!dirty) return;
            e.preventDefault();
            e.returnValue = 'You have unsaved changes. Leave anyway?';
        });
    });

    /* ──────────────────────────────────────────────────────────
       12. Row-click navigation for tables
       Add data-href="/url" to <tr> to make entire row clickable
    ────────────────────────────────────────────────────────── */
    document.querySelectorAll('tr[data-href]').forEach(tr => {
        tr.style.cursor = 'pointer';
        tr.addEventListener('click', function (e) {
            if (e.target.closest('a, button, .btn, input, select')) return;
            window.location.href = this.dataset.href;
        });
    });

    /* ──────────────────────────────────────────────────────────
       13. Smooth page transition (fade on navigate)
    ────────────────────────────────────────────────────────── */
    document.addEventListener('click', e => {
        const anchor = e.target.closest('a[href]');
        if (!anchor) return;
        if (anchor.closest('.no-page-transition')) return;
        const href = anchor.getAttribute('href');
        // Only local, non-hash, non-external links
        if (!href || href.startsWith('#') || href.startsWith('http') ||
            anchor.target === '_blank' || e.metaKey || e.ctrlKey) return;

        e.preventDefault();
        document.body.style.transition = 'opacity .2s ease';
        document.body.style.opacity = '0';
        setTimeout(() => window.location.href = href, 200);
    });

    window.addEventListener('pageshow', () => {
        document.body.style.transition = 'opacity .25s ease';
        document.body.style.opacity = '1';
    });

    /* ──────────────────────────────────────────────────────────
       14. Tooltips: wire up Bootstrap tooltips globally
    ────────────────────────────────────────────────────────── */
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        document.querySelectorAll('[data-bs-toggle="tooltip"]')
            .forEach(el => new bootstrap.Tooltip(el, { trigger: 'hover' }));
    }

    /* ──────────────────────────────────────────────────────────
       15. Expose convenience helpers on window
    ────────────────────────────────────────────────────────── */

    /** Quick confirmation wrapper that returns a Promise */
    window.confirmAction = function (message = 'Are you sure?') {
        return new Promise(resolve => {
            // Use native confirm; can be swapped for a modal variant
            resolve(window.confirm(message));
        });
    };

    /** Programmatically toggle dark mode */
    window.toggleDarkMode = function () {
        const html = document.documentElement;
        const next = html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-bs-theme', next);
        localStorage.setItem('theme', next);
    };

    /** Restore persisted theme on load */
    (function restoreTheme() {
        const saved = localStorage.getItem('theme');
        if (saved) document.documentElement.setAttribute('data-bs-theme', saved);
    })();

})();