/**
 * landing.js — Public home hero effects
 * Scroll parallax · pointer parallax · optional hooks for future libraries
 *
 * Extend via window.OnSetLanding.init({ ... }) after DOM ready.
 */
(function () {
    'use strict';

    const hero = document.querySelector('[data-landing-hero]');
    if (!hero) return;

    const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const parallaxLayers = hero.querySelectorAll('[data-parallax-depth]');
    const orbs = hero.querySelectorAll('.landing-orb');

    let pointerX = 0;
    let pointerY = 0;
    let scrollY = 0;
    let rafId = null;

    function applyParallax() {
        parallaxLayers.forEach(function (layer) {
            const depth = parseFloat(layer.getAttribute('data-parallax-depth')) || 0.15;
            const scrollOffset = scrollY * depth;
            const pointerOffsetX = pointerX * depth * 48;
            const pointerOffsetY = pointerY * depth * 48;
            layer.style.transform =
                'translate3d(' + pointerOffsetX + 'px, ' + (scrollOffset + pointerOffsetY) + 'px, 0)';
        });

        if (!reducedMotion) {
            orbs.forEach(function (orb, index) {
                const factor = (index + 1) * 0.12;
                orb.style.translate =
                    (pointerX * factor * 24) + 'px ' + (pointerY * factor * 24 + scrollY * factor * 0.3) + 'px';
            });
        }
    }

    function scheduleUpdate() {
        if (rafId !== null) return;
        rafId = requestAnimationFrame(function () {
            applyParallax();
            rafId = null;
        });
    }

    function onScroll() {
        scrollY = window.scrollY;
        scheduleUpdate();
    }

    function onPointerMove(e) {
        const rect = hero.getBoundingClientRect();
        pointerX = (e.clientX - rect.left) / rect.width - 0.5;
        pointerY = (e.clientY - rect.top) / rect.height - 0.5;
        scheduleUpdate();
    }

    function onPointerLeave() {
        pointerX = 0;
        pointerY = 0;
        scheduleUpdate();
    }

    if (!reducedMotion) {
        window.addEventListener('scroll', onScroll, { passive: true });
        hero.addEventListener('mousemove', onPointerMove, { passive: true });
        hero.addEventListener('mouseleave', onPointerLeave, { passive: true });
        onScroll();
    }

    /** Public extension point for GSAP, Lottie, video backgrounds, etc. */
    window.OnSetLanding = window.OnSetLanding || {};
    window.OnSetLanding.init = function (options) {
        if (options && typeof options.onReady === 'function') {
            options.onReady(hero);
        }
    };

    window.OnSetLanding.hero = hero;
    window.OnSetLanding.refresh = scheduleUpdate;

    document.dispatchEvent(new CustomEvent('onset-landing-ready', { detail: { hero: hero } }));
})();
