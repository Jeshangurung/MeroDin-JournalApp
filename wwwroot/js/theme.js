window.setTheme = (theme) => {
    document.body.classList.remove('theme-light', 'theme-dark');
    document.body.classList.add('theme-' + theme);
    
    // Also update the html tag for good measure
    document.documentElement.classList.remove('theme-light', 'theme-dark');
    document.documentElement.classList.add('theme-' + theme);

    // Support for Quill editor if present
    const quillEditors = document.querySelectorAll('.ql-container, .ql-toolbar');
    quillEditors.forEach(el => {
        if (theme === 'dark') {
            el.classList.add('ql-dark');
        } else {
            el.classList.remove('ql-dark');
        }
    });
};
