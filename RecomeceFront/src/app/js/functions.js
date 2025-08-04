const SetInnerHTMl = (id, content) => {
    const element = document.getElementById(id);
    if (id)
        element.innerHTML = content;
}

const ClickElement = (id) => {
    const el = document.getElementById(id);
    el?.click();
}

const copyText = (texto) => {
    const textArea = document.createElement('textarea');
    textArea.value = texto;
    document.body.appendChild(textArea);
    textArea.select();
    document.execCommand('copy');
    document.body.removeChild(textArea);
}

const copyTextAtt = (el) => {
    const textArea = document.createElement('textarea');
    textArea.value = el.getAttribute('data-text');
    document.body.appendChild(textArea);
    textArea.select();
    document.execCommand('copy');
    document.body.removeChild(textArea);
}

const ValidaData = (el) => {
    let s = el.value.split('/');
    const dt = `${s[2].substring(0, 4)}-${s[1]}-${s[0]}`
    try {
        const data = new Date(dt);
        if (data == 'Invalid Date')
            el.value = ''
    }
    catch {
        el.value = ''
    }
}
const ToogleBackDrop = () => {
    const btn = document.getElementById('BtnBackDrop');
    if (btn) {
        btn.click();
    }
}

const ToogleBackDrop2 = () => {
    const div = document.getElementById('back-drop');
    if (div) {
        div.parentNode.removeChild(div);
    }
    else {
        const newDiv = document.createElement('div');
        newDiv.id = 'back-drop';
        newDiv.setAttribute('onclick', 'ToogleBackDrop()');
        newDiv.style.zIndex = '1055';
        newDiv.setAttribute('aria-hidden', 'true');
        newDiv.className = 'modal-backdrop fade show';

        // Adiciona o elemento ao body
        document.body.appendChild(newDiv);
    }
}