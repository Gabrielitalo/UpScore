import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class XmlParserService {

constructor() { }


  /**
   * Converte string XML em objeto JSON
   * @param xmlString XML em string
   */
  public xmlToJson(xmlString: string): any {
    const parser = new DOMParser();
    const xml = parser.parseFromString(xmlString, 'application/xml');
    return this.nodeToJson(xml.documentElement);
  }

  /**
   * Conversão recursiva de XML para JSON
   */
  private nodeToJson(xmlNode: any): any {
    const obj: any = {};

    // Atributos
    if (xmlNode.attributes?.length > 0) {
      obj['@attributes'] = {};
      let attr: any;
      for (attr of Array.from(xmlNode.attributes)) {
        obj['@attributes'][attr.nodeName] = attr.nodeValue;
      }
    }

    // Filhos
    if (xmlNode.hasChildNodes()) {
      let node: any;
      for (node of Array.from(xmlNode.childNodes)) {
        const nodeName = node.nodeName.replace(/^.*:/, ''); // Remove prefixo
        const value = this.nodeToJson(node);

        // Texto direto
        if (node.nodeType === 3 && node.nodeValue.trim() !== '') {
          return node.nodeValue.trim();
        }

        if (!value || value === '') continue;

        if (obj[nodeName] === undefined) {
          obj[nodeName] = value;
        } else {
          if (!Array.isArray(obj[nodeName])) {
            obj[nodeName] = [obj[nodeName]];
          }
          obj[nodeName].push(value);
        }
      }
    }

    return obj;
  }

}
