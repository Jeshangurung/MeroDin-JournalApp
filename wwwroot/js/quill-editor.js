let quill;

window.initQuill = (editorId, dotNetHelper) => {
    quill = new Quill(`#${editorId}`, {
        theme: "snow",
        modules: {
            toolbar: [
                ["bold", "italic", "underline"],
                [{ list: "ordered" }, { list: "bullet" }],
                [{ header: [1, 2, false] }],
                ["clean"]
            ]
        }
    });

    quill.on('text-change', () => {
        dotNetHelper.invokeMethodAsync('UpdateContent', quill.root.innerHTML);
    });
};

window.getQuillHtml = () => {
    return quill.root.innerHTML;
};

window.setQuillHtml = (html) => {
    quill.root.innerHTML = html;
};