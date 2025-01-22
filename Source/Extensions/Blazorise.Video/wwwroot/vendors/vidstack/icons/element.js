import { lazyPaths } from './lazy.js';

function createTemplate(content) {
  const template = document.createElement("template");
  template.innerHTML = content;
  return template.content;
}
function cloneTemplateContent(content) {
  const fragment = content.cloneNode(true);
  return fragment.firstElementChild;
}

const svgTemplate = /* @__PURE__ */ createTemplate(
  `<svg width="100%" height="100%" viewBox="0 0 32 32" fill="none" aria-hidden="true" focusable="false" xmlns="http://www.w3.org/2000/svg"></svg>`
);
class MediaIconElement extends HTMLElement {
  constructor() {
    super(...arguments);
    this._svg = this._createSVG();
    this._type = null;
  }
  static {
    this.tagName = "media-icon";
  }
  static get observedAttributes() {
    return ["type"];
  }
  /**
   * The type of icon. You can find a complete and searchable list on our website - see our
   * [media icons catalog](https://www.vidstack.io/media-icons?lib=html).
   */
  get type() {
    return this._type;
  }
  set type(type) {
    if (this._type === type)
      return;
    if (type)
      this.setAttribute("type", type);
    else
      this.removeAttribute("type");
    this._onTypeChange(type);
  }
  attributeChangedCallback(name, _, newValue) {
    if (name === "type") {
      const type = newValue ? newValue : null;
      if (this._type !== type) {
        this._onTypeChange(type);
      }
    }
  }
  connectedCallback() {
    this.classList.add("vds-icon");
    if (this._svg.parentNode !== this) {
      this.prepend(this._svg);
    }
  }
  _createSVG() {
    return cloneTemplateContent(svgTemplate);
  }
  _loadIcon() {
    const type = this._type;
    if (type && lazyPaths[type]) {
      lazyPaths[type]().then(({ default: paths }) => {
        if (type === this._type)
          this._onPathsChange(paths);
      });
    } else {
      this._onPathsChange("");
    }
  }
  _onTypeChange(type) {
    this._type = type;
    this._loadIcon();
  }
  _onPathsChange(paths) {
    this._svg.innerHTML = paths;
  }
}
if (!window.customElements.get(MediaIconElement.tagName)) {
  window.customElements.define(MediaIconElement.tagName, MediaIconElement);
}

export { MediaIconElement };
