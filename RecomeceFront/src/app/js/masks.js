const formatDate = (input) => {
  let v = input.value.replace(/\D/g, ''); // remove tudo que não é número

  if (v.length > 2 && v.length <= 4) {
    v = v.slice(0, 2) + '/' + v.slice(2);
  } else if (v.length > 4) {
    v = v.slice(0, 2) + '/' + v.slice(2, 4) + '/' + v.slice(4, 8);
  }

  input.value = v;
}
const formatDateTime = (input) => {
  let v = input.value;
  let l = input.value.length;

  switch (l) {
    case 2:
      input.value = v + "/";
      break;
    case 5:
      input.value = v + "/";
      break;
    case 10:
      input.value = v + " ";
      break;
    case 13:
      input.value = v + ":";
      break;
    case 17:
      input.value = v + ":";
      break;
  }
}

const formatarMoeda = (event) => {
  const input = event.target;
  const valor = input.value.replace(/\D/g, '').replace(/([0-9]{2})$/, ',$1');
  input.value = valor.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
}

const MascaraTelefone = (el) => {
  let v = el.value;
  if (!v)
    return '';
  var r = v.replace(/\D/g, "");
  r = r.replace(/^0/, "");
  if (r.length > 10) {
    r = r.replace(/^(\d\d)(\d{5})(\d{4}).*/, "($1) $2-$3");
  } else if (r.length > 5) {
    r = r.replace(/^(\d\d)(\d{4})(\d{0,4}).*/, "($1) $2-$3");
  } else if (r.length > 2) {
    r = r.replace(/^(\d\d)(\d{0,5})/, "($1) $2");
  } else {
    r = r.replace(/^(\d*)/, "($1");
  }
  el.value = r;
}

const MascaraCnpjCpf = (el) => {
  let s = el.value;
  // if (!s) return '';
  // if (s.length == 14)
  //   s = s.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, "$1.$2.$3/$4-$5")
  // else if (s.length == 11)
  //   s = s.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4")
  const raw = el.value.replace(/\D/g, '');

  let masked = raw;

  if (raw.length <= 11) {
    // CPF: 999.999.999-99
    masked = raw
      .replace(/^(\d{3})(\d)/, '$1.$2')
      .replace(/^(\d{3})\.(\d{3})(\d)/, '$1.$2.$3')
      .replace(/^(\d{3})\.(\d{3})\.(\d{3})(\d{1,2})?$/, '$1.$2.$3-$4');
  } else {
    // CNPJ: 99.999.999/9999-99
    masked = raw
      .replace(/^(\d{2})(\d)/, '$1.$2')
      .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
      .replace(/^(\d{2})\.(\d{3})\.(\d{3})(\d)/, '$1.$2.$3/$4')
      .replace(/^(\d{2})\.(\d{3})\.(\d{3})\/(\d{4})(\d{1,2})?$/, '$1.$2.$3/$4-$5');
  }

  el.value = masked;
}

const MascaraCpf = (el) => {
  let s = el.value;
  if (!s) return '';
  if (s.length == 11)
    s = s.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4")
  el.value = s;
}

const MascaraCnpj = (el) => {
  let s = el.value;
  if (!s) return '';
  if (s.length == 14)
    s = s.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, "$1.$2.$3/$4-$5")
  el.value = s;
}

const MascaraCEP = (el) => {
  const raw = el.value.replace(/\D/g, '');
  // Aplica a máscara 99999-999
  const masked = raw.length <= 5
    ? raw
    : `${raw.substring(0, 5)}-${raw.substring(5, 8)}`;

  el.value = masked;
}
