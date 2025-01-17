import{u as jt,c as Yt,M as pt,L as Zt,z as b,O as Xt,s as h,a0 as Jt,e as C,n as G,a9 as i,P as mt,aa as te,i as P,ad as R,T as ee,ae as W,m as H,Q as F,o as ae,k as se,ac as ne,a as A,t as ie,p as oe,b as le,af as re,v as ue,R as vt,l as j,ag as de,U as ft}from"./vidstack-9sLhInZa.js";import{u as k,w as $t,a as v,$ as ce,s as pe,b as me,c as bt,d as ve,i as fe,L as gt,e as $e}from"./vidstack-7n6mvK4o.js";import{$ as n,L as be,I as ge,S as ht}from"./vidstack-CdxIGwEZ.js";import{b as he,a as ye}from"./vidstack-CCLEdTWF.js";import{b as yt}from"./vidstack-49LFp0xO.js";import"./vidstack-aGbemMlX.js";import"./vidstack-R9a36ywQ.js";import"./vidstack-C_AxqLKV.js";import"./vidstack-DRH_1tFW.js";import"./vidstack-BfBBPhXV.js";import"./vidstack-Bxv1Qnxe.js";const xt=Yt();function r(){return jt(xt)}const xe={colorScheme:"system",download:null,customIcons:!1,disableTimeSlider:!1,menuContainer:null,menuGroup:"bottom",noAudioGain:!1,noGestures:!1,noKeyboardAnimations:!1,noModal:!1,noScrubGesture:!1,playbackRates:{min:0,max:2,step:.25},audioGains:{min:0,max:300,step:25},seekStep:10,sliderChaptersMinWidth:325,hideQualityBitrate:!1,smallWhen:!1,thumbnails:null,translations:null,when:!1};class Y extends Zt{static props=xe;#t;#e=b(()=>{const t=this.$props.when();return this.#s(t)});#a=b(()=>{const t=this.$props.smallWhen();return this.#s(t)});get isMatch(){return this.#e()}get isSmallLayout(){return this.#a()}onSetup(){this.#t=k(),this.setAttributes({"data-match":this.#e,"data-sm":()=>this.#a()?"":null,"data-lg":()=>this.#a()?null:"","data-size":()=>this.#a()?"sm":"lg","data-no-scrub-gesture":this.$props.noScrubGesture}),Xt(xt,{...this.$props,when:this.#e,smallWhen:this.#a,userPrefersAnnouncements:h(!0),userPrefersKeyboardAnimations:h(!0),menuPortal:h(null)})}onAttach(t){$t(t,this.$props.colorScheme)}#s(t){return t!=="never"&&(Jt(t)?t:b(()=>t(this.#t.player.state))())}}const St=Y.prototype;pt(St,"isMatch"),pt(St,"isSmallLayout");let Se=class extends Y{static props={...super.props,when:({viewType:t})=>t==="audio",smallWhen:({width:t})=>t<576}};function kt(e,t){C(()=>{const{player:a}=k(),s=a.el;return s&&G(s,"data-layout",t()&&e),()=>s?.removeAttribute("data-layout")})}function S(e,t){return e()?.[t]??t}function Z(){return n(()=>{const{translations:e,userPrefersAnnouncements:t}=r();return t()?i`<media-announcer .translations=${n(e)}></media-announcer>`:null})}function w(e,t=""){return i`<slot
    name=${`${e}-icon`}
    data-class=${`vds-icon vds-${e}-icon${t?` ${t}`:""}`}
  ></slot>`}function V(e){return e.map(t=>w(t))}function d(e,t){return n(()=>S(e,t))}function X({tooltip:e}){const{translations:t}=r(),{remotePlaybackState:a}=v(),s=n(()=>{const l=S(t,"AirPlay"),u=mt(a());return`${l} ${u}`}),o=d(t,"AirPlay");return i`
    <media-tooltip class="vds-airplay-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-airplay-button class="vds-airplay-button vds-button" aria-label=${s}>
          ${w("airplay")}
        </media-airplay-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content" placement=${e}>
        <span class="vds-airplay-tooltip-text">${o}</span>
      </media-tooltip-content>
    </media-tooltip>
  `}function wt({tooltip:e}){const{translations:t}=r(),{remotePlaybackState:a}=v(),s=n(()=>{const l=S(t,"Google Cast"),u=mt(a());return`${l} ${u}`}),o=d(t,"Google Cast");return i`
    <media-tooltip class="vds-google-cast-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-google-cast-button class="vds-google-cast-button vds-button" aria-label=${s}>
          ${w("google-cast")}
        </media-google-cast-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content" placement=${e}>
        <span class="vds-google-cast-tooltip-text">${o}</span>
      </media-tooltip-content>
    </media-tooltip>
  `}function _({tooltip:e}){const{translations:t}=r(),a=d(t,"Play"),s=d(t,"Pause");return i`
    <media-tooltip class="vds-play-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-play-button
          class="vds-play-button vds-button"
          aria-label=${d(t,"Play")}
        >
          ${V(["play","pause","replay"])}
        </media-play-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content" placement=${e}>
        <span class="vds-play-tooltip-text">${a}</span>
        <span class="vds-pause-tooltip-text">${s}</span>
      </media-tooltip-content>
    </media-tooltip>
  `}function Tt({tooltip:e,ref:t=ee}){const{translations:a}=r(),s=d(a,"Mute"),o=d(a,"Unmute");return i`
    <media-tooltip class="vds-mute-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-mute-button
          class="vds-mute-button vds-button"
          aria-label=${d(a,"Mute")}
          ${R(t)}
        >
          ${V(["mute","volume-low","volume-high"])}
        </media-mute-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content" placement=${e}>
        <span class="vds-mute-tooltip-text">${o}</span>
        <span class="vds-unmute-tooltip-text">${s}</span>
      </media-tooltip-content>
    </media-tooltip>
  `}function J({tooltip:e}){const{translations:t}=r(),a=d(t,"Closed-Captions On"),s=d(t,"Closed-Captions Off");return i`
    <media-tooltip class="vds-caption-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-caption-button
          class="vds-caption-button vds-button"
          aria-label=${d(t,"Captions")}
        >
          ${V(["cc-on","cc-off"])}
        </media-caption-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content" placement=${e}>
        <span class="vds-cc-on-tooltip-text">${s}</span>
        <span class="vds-cc-off-tooltip-text">${a}</span>
      </media-tooltip-content>
    </media-tooltip>
  `}function ke(){const{translations:e}=r(),t=d(e,"Enter PiP"),a=d(e,"Exit PiP");return i`
    <media-tooltip class="vds-pip-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-pip-button
          class="vds-pip-button vds-button"
          aria-label=${d(e,"PiP")}
        >
          ${V(["pip-enter","pip-exit"])}
        </media-pip-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content">
        <span class="vds-pip-enter-tooltip-text">${t}</span>
        <span class="vds-pip-exit-tooltip-text">${a}</span>
      </media-tooltip-content>
    </media-tooltip>
  `}function Ct({tooltip:e}){const{translations:t}=r(),a=d(t,"Enter Fullscreen"),s=d(t,"Exit Fullscreen");return i`
    <media-tooltip class="vds-fullscreen-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-fullscreen-button
          class="vds-fullscreen-button vds-button"
          aria-label=${d(t,"Fullscreen")}
        >
          ${V(["fs-enter","fs-exit"])}
        </media-fullscreen-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content" placement=${e}>
        <span class="vds-fs-enter-tooltip-text">${a}</span>
        <span class="vds-fs-exit-tooltip-text">${s}</span>
      </media-tooltip-content>
    </media-tooltip>
  `}function Dt({backward:e,tooltip:t}){const{translations:a,seekStep:s}=r(),o=e?"Seek Backward":"Seek Forward",l=d(a,o);return i`
    <media-tooltip class="vds-seek-tooltip vds-tooltip">
      <media-tooltip-trigger>
        <media-seek-button
          class="vds-seek-button vds-button"
          seconds=${n(()=>(e?-1:1)*s())}
          aria-label=${l}
        >
          ${w(e?"seek-backward":"seek-forward")}
        </media-seek-button>
      </media-tooltip-trigger>
      <media-tooltip-content class="vds-tooltip-content" placement=${t}>
        ${d(a,o)}
      </media-tooltip-content>
    </media-tooltip>
  `}function Mt(){const{translations:e}=r(),{live:t}=v(),a=d(e,"Skip To Live"),s=d(e,"LIVE");return t()?i`
        <media-live-button class="vds-live-button" aria-label=${a}>
          <span class="vds-live-button-text">${s}</span>
        </media-live-button>
      `:null}function tt(){return n(()=>{const{download:e,translations:t}=r(),a=e();if(te(a))return null;const{source:s,title:o}=v(),l=s(),u=he({title:o(),src:l,download:a});return P(u?.url)?i`
          <media-tooltip class="vds-download-tooltip vds-tooltip">
            <media-tooltip-trigger>
              <a
                role="button"
                class="vds-download-button vds-button"
                aria-label=${d(t,"Download")}
                href=${ye(u.url,{download:u.name})}
                download=${u.name}
                target="_blank"
              >
                <slot name="download-icon" data-class="vds-icon" />
              </a>
            </media-tooltip-trigger>
            <media-tooltip-content class="vds-tooltip-content" placement="top">
              ${d(t,"Download")}
            </media-tooltip-content>
          </media-tooltip>
        `:null})}function et(){const{translations:e}=r();return i`
    <media-captions
      class="vds-captions"
      .exampleText=${d(e,"Captions look like this")}
    ></media-captions>
  `}function D(){return i`<div class="vds-controls-spacer"></div>`}function At(e,t){return i`
    <media-menu-portal .container=${n(e)} disabled="fullscreen">
      ${t}
    </media-menu-portal>
  `}function Ot(e,t,a,s){let o=P(t)?document.querySelector(t):t;o||(o=e?.closest("dialog")),o||(o=document.body);const l=document.createElement("div");l.style.display="contents",l.classList.add(a),o.append(l),C(()=>{if(!l)return;const{viewType:p}=v(),c=s();G(l,"data-view-type",p()),G(l,"data-sm",c),G(l,"data-lg",!c),G(l,"data-size",c?"sm":"lg")});const{colorScheme:u}=r();return $t(l,u),l}function It({placement:e,tooltip:t,portal:a}){const{textTracks:s}=k(),{viewType:o,seekableStart:l,seekableEnd:u}=v(),{translations:p,thumbnails:c,menuPortal:$,noModal:f,menuGroup:m,smallWhen:g}=r();if(b(()=>{const Ut=l(),Ht=u(),dt=h(null);return yt(s,"chapters",dt.set),!dt()?.cues.filter(ct=>ct.startTime<=Ht&&ct.endTime>=Ut)?.length})())return null;const y=b(()=>f()?W(e):g()?null:W(e)),L=b(()=>!g()&&m()==="bottom"&&o()==="video"?26:0),B=h(!1);function zt(){B.set(!0)}function qt(){B.set(!1)}const ut=i`
    <media-menu-items
      class="vds-chapters-menu-items vds-menu-items"
      placement=${n(y)}
      offset=${n(L)}
    >
      ${n(()=>B()?i`
          <media-chapters-radio-group
            class="vds-chapters-radio-group vds-radio-group"
            .thumbnails=${n(c)}
          >
            <template>
              <media-radio class="vds-chapter-radio vds-radio">
                <media-thumbnail class="vds-thumbnail"></media-thumbnail>
                <div class="vds-chapter-radio-content">
                  <span class="vds-chapter-radio-label" data-part="label"></span>
                  <span class="vds-chapter-radio-start-time" data-part="start-time"></span>
                  <span class="vds-chapter-radio-duration" data-part="duration"></span>
                </div>
              </media-radio>
            </template>
          </media-chapters-radio-group>
        `:null)}
    </media-menu-items>
  `;return i`
    <media-menu class="vds-chapters-menu vds-menu" @open=${zt} @close=${qt}>
      <media-tooltip class="vds-tooltip">
        <media-tooltip-trigger>
          <media-menu-button
            class="vds-menu-button vds-button"
            aria-label=${d(p,"Chapters")}
          >
            ${w("menu-chapters")}
          </media-menu-button>
        </media-tooltip-trigger>
        <media-tooltip-content
          class="vds-tooltip-content"
          placement=${H(t)?n(t):t}
        >
          ${d(p,"Chapters")}
        </media-tooltip-content>
      </media-tooltip>
      ${a?At($,ut):ut}
    </media-menu>
  `}function at(e){const{style:t}=new Option;return t.color=e,t.color.match(/\((.*?)\)/)[1].replace(/,/g," ")}const st={type:"color"},we={type:"radio",values:{"Monospaced Serif":"mono-serif","Proportional Serif":"pro-serif","Monospaced Sans-Serif":"mono-sans","Proportional Sans-Serif":"pro-sans",Casual:"casual",Cursive:"cursive","Small Capitals":"capitals"}},Te={type:"slider",min:0,max:400,step:25,upIcon:null,downIcon:null},Ce={type:"slider",min:0,max:100,step:5,upIcon:null,downIcon:null},De={type:"radio",values:["None","Drop Shadow","Raised","Depressed","Outline"]},E={fontFamily:"pro-sans",fontSize:"100%",textColor:"#ffffff",textOpacity:"100%",textShadow:"none",textBg:"#000000",textBgOpacity:"100%",displayBg:"#000000",displayBgOpacity:"0%"},O=Object.keys(E).reduce((e,t)=>({...e,[t]:h(E[t])}),{});for(const e of Object.keys(O)){const t=localStorage.getItem(`vds-player:${F(e)}`);P(t)&&O[e].set(t)}function Me(){for(const e of Object.keys(O)){const t=E[e];O[e].set(t)}}let Pt=!1,nt=new Set;function Ae(){const{player:e}=k();nt.add(e),ae(()=>nt.delete(e)),Pt||(se(()=>{for(const t of ne(O)){const a=O[t],s=E[t],o=`--media-user-${F(t)}`,l=`vds-player:${F(t)}`;C(()=>{const u=a(),p=u===s,c=p?null:Oe(e,t,u);for(const $ of nt)$.el?.style.setProperty(o,c);p?localStorage.removeItem(l):localStorage.setItem(l,u)})}},null),Pt=!0)}function Oe(e,t,a){switch(t){case"fontFamily":const s=a==="capitals"?"small-caps":"";return e.el?.style.setProperty("--media-user-font-variant",s),Pe(a);case"fontSize":case"textOpacity":case"textBgOpacity":case"displayBgOpacity":return Ie(a);case"textColor":return`rgb(${at(a)} / var(--media-user-text-opacity, 1))`;case"textShadow":return Le(a);case"textBg":return`rgb(${at(a)} / var(--media-user-text-bg-opacity, 1))`;case"displayBg":return`rgb(${at(a)} / var(--media-user-display-bg-opacity, 1))`}}function Ie(e){return(parseInt(e)/100).toString()}function Pe(e){switch(e){case"mono-serif":return'"Courier New", Courier, "Nimbus Mono L", "Cutive Mono", monospace';case"mono-sans":return'"Deja Vu Sans Mono", "Lucida Console", Monaco, Consolas, "PT Mono", monospace';case"pro-sans":return'Roboto, "Arial Unicode Ms", Arial, Helvetica, Verdana, "PT Sans Caption", sans-serif';case"casual":return'"Comic Sans MS", Impact, Handlee, fantasy';case"cursive":return'"Monotype Corsiva", "URW Chancery L", "Apple Chancery", "Dancing Script", cursive';case"capitals":return'"Arial Unicode Ms", Arial, Helvetica, Verdana, "Marcellus SC", sans-serif + font-variant=small-caps';default:return'"Times New Roman", Times, Georgia, Cambria, "PT Serif Caption", serif'}}function Le(e){switch(e){case"drop shadow":return"rgb(34, 34, 34) 1.86389px 1.86389px 2.79583px, rgb(34, 34, 34) 1.86389px 1.86389px 3.72778px, rgb(34, 34, 34) 1.86389px 1.86389px 4.65972px";case"raised":return"rgb(34, 34, 34) 1px 1px, rgb(34, 34, 34) 2px 2px";case"depressed":return"rgb(204, 204, 204) 1px 1px, rgb(34, 34, 34) -1px -1px";case"outline":return"rgb(34, 34, 34) 0px 0px 1.86389px, rgb(34, 34, 34) 0px 0px 1.86389px, rgb(34, 34, 34) 0px 0px 1.86389px, rgb(34, 34, 34) 0px 0px 1.86389px, rgb(34, 34, 34) 0px 0px 1.86389px";default:return""}}let Be=0;function T({label:e="",value:t="",children:a}){if(!e)return i`
      <div class="vds-menu-section">
        <div class="vds-menu-section-body">${a}</div>
      </div>
    `;const s=`vds-menu-section-${++Be}`;return i`
    <section class="vds-menu-section" role="group" aria-labelledby=${s}>
      <div class="vds-menu-section-title">
        <header id=${s}>${e}</header>
        ${t?i`<div class="vds-menu-section-value">${t}</div>`:null}
      </div>
      <div class="vds-menu-section-body">${a}</div>
    </section>
  `}function N({label:e,children:t}){return i`
    <div class="vds-menu-item">
      <div class="vds-menu-item-label">${e}</div>
      ${t}
    </div>
  `}function I({label:e,icon:t,hint:a}){return i`
    <media-menu-button class="vds-menu-item">
      ${w("menu-arrow-left","vds-menu-close-icon")}
      ${t?w(t,"vds-menu-item-icon"):null}
      <span class="vds-menu-item-label">${n(e)}</span>
      <span class="vds-menu-item-hint" data-part="hint">${a?n(a):null} </span>
      ${w("menu-arrow-right","vds-menu-open-icon")}
    </media-menu-button>
  `}function Ge({value:e=null,options:t,hideLabel:a=!1,children:s=null,onChange:o=null}){function l(u){const{value:p,label:c}=u;return i`
      <media-radio class="vds-radio" value=${p}>
        ${w("menu-radio-check")}
        ${a?null:i`
              <span class="vds-radio-label" data-part="label">
                ${P(c)?c:n(c)}
              </span>
            `}
        ${H(s)?s(u):s}
      </media-radio>
    `}return i`
    <media-radio-group
      class="vds-radio-group"
      value=${P(e)?e:e?n(e):""}
      @change=${o}
    >
      ${A(t)?t.map(l):n(()=>t().map(l))}
    </media-radio-group>
  `}function Fe(e){return A(e)?e.map(t=>({label:t,value:t.toLowerCase()})):Object.keys(e).map(t=>({label:t,value:e[t]}))}function K(){return i`
    <div class="vds-slider-track"></div>
    <div class="vds-slider-track-fill vds-slider-track"></div>
    <div class="vds-slider-thumb"></div>
  `}function Q(){return i`
    <media-slider-steps class="vds-slider-steps">
      <template>
        <div class="vds-slider-step"></div>
      </template>
    </media-slider-steps>
  `}function z({label:e=null,value:t=null,upIcon:a="",downIcon:s="",children:o,isMin:l,isMax:u}){const p=e||t,c=[s?w(s,"down"):null,o,a?w(a,"up"):null];return i`
    <div
      class=${`vds-menu-item vds-menu-slider-item${p?" group":""}`}
      data-min=${n(()=>l()?"":null)}
      data-max=${n(()=>u()?"":null)}
    >
      ${p?i`
            <div class="vds-menu-slider-title">
              ${[e?i`<div>${e}</div>`:null,t?i`<div>${t}</div>`:null]}
            </div>
            <div class="vds-menu-slider-body">${c}</div>
          `:c}
    </div>
  `}const Ve={...Te,upIcon:"menu-opacity-up",downIcon:"menu-opacity-down"},it={...Ce,upIcon:"menu-opacity-up",downIcon:"menu-opacity-down"};function Ne(){return n(()=>{const{hasCaptions:e}=v(),{translations:t}=r();return e()?i`
      <media-menu class="vds-font-menu vds-menu">
        ${I({label:()=>S(t,"Caption Styles")})}
        <media-menu-items class="vds-menu-items">
          ${[T({label:d(t,"Font"),children:[Re(),We()]}),T({label:d(t,"Text"),children:[_e(),Ke(),Ee()]}),T({label:d(t,"Text Background"),children:[Qe(),ze()]}),T({label:d(t,"Display Background"),children:[qe(),Ue()]}),T({children:[He()]})]}
        </media-menu-items>
      </media-menu>
    `:null})}function Re(){return M({label:"Family",option:we,type:"fontFamily"})}function We(){return M({label:"Size",option:Ve,type:"fontSize"})}function _e(){return M({label:"Color",option:st,type:"textColor"})}function Ee(){return M({label:"Opacity",option:it,type:"textOpacity"})}function Ke(){return M({label:"Shadow",option:De,type:"textShadow"})}function Qe(){return M({label:"Color",option:st,type:"textBg"})}function ze(){return M({label:"Opacity",option:it,type:"textBgOpacity"})}function qe(){return M({label:"Color",option:st,type:"displayBg"})}function Ue(){return M({label:"Opacity",option:it,type:"displayBgOpacity"})}function He(){const{translations:e}=r();return i`
    <button class="vds-menu-item" role="menuitem" @click=${Me}>
      <span class="vds-menu-item-label">${n(()=>S(e,"Reset"))}</span>
    </button>
  `}function M({label:e,option:t,type:a}){const{player:s}=k(),{translations:o}=r(),l=O[a],u=()=>S(o,e);function p(){ie(),s.dispatchEvent(new Event("vds-font-change"))}if(t.type==="color"){let f=function(m){l.set(m.target.value),p()};return N({label:n(u),children:i`
        <input
          class="vds-color-picker"
          type="color"
          .value=${n(l)}
          @input=${f}
        />
      `})}if(t.type==="slider"){let f=function(B){l.set(B.detail+"%"),p()};const{min:m,max:g,step:x,upIcon:y,downIcon:L}=t;return z({label:n(u),value:n(l),upIcon:y,downIcon:L,isMin:()=>l()===m+"%",isMax:()=>l()===g+"%",children:i`
        <media-slider
          class="vds-slider"
          min=${m}
          max=${g}
          step=${x}
          key-step=${x}
          .value=${n(()=>parseInt(l()))}
          aria-label=${n(u)}
          @value-change=${f}
          @drag-value-change=${f}
        >
          ${K()}${Q()}
        </media-slider>
      `})}const c=Fe(t.values),$=()=>{const f=l(),m=c.find(g=>g.value===f)?.label||"";return S(o,P(m)?m:m())};return i`
    <media-menu class=${`vds-${F(a)}-menu vds-menu`}>
      ${I({label:u,hint:$})}
      <media-menu-items class="vds-menu-items">
        ${Ge({value:l,options:c,onChange({detail:f}){l.set(f),p()}})}
      </media-menu-items>
    </media-menu>
  `}function q({label:e,checked:t,defaultChecked:a=!1,storageKey:s,onChange:o}){const{translations:l}=r(),u=s?localStorage.getItem(s):null,p=h(!!(u??a)),c=h(!1),$=n(ce(p)),f=d(l,e);s&&o(oe(p)),t&&C(()=>void p.set(t()));function m(y){y?.button!==1&&(p.set(L=>!L),s&&localStorage.setItem(s,p()?"1":""),o(p(),y),c.set(!1))}function g(y){le(y)&&m()}function x(y){y.button===0&&c.set(!0)}return i`
    <div
      class="vds-menu-checkbox"
      role="menuitemcheckbox"
      tabindex="0"
      aria-label=${f}
      aria-checked=${$}
      data-active=${n(()=>c()?"":null)}
      @pointerup=${m}
      @pointerdown=${x}
      @keydown=${g}
    ></div>
  `}function je(){return n(()=>{const{translations:e}=r();return i`
      <media-menu class="vds-accessibility-menu vds-menu">
        ${I({label:()=>S(e,"Accessibility"),icon:"menu-accessibility"})}
        <media-menu-items class="vds-menu-items">
          ${[T({children:[Ye(),Ze()]}),T({children:[Ne()]})]}
        </media-menu-items>
      </media-menu>
    `})}function Ye(){const{userPrefersAnnouncements:e,translations:t}=r(),a="Announcements";return N({label:d(t,a),children:q({label:a,storageKey:"vds-player::announcements",onChange(s){e.set(s)}})})}function Ze(){return n(()=>{const{translations:e,userPrefersKeyboardAnimations:t,noKeyboardAnimations:a}=r(),{viewType:s}=v();if(b(()=>s()!=="video"||a())())return null;const l="Keyboard Animations";return N({label:d(e,l),children:q({label:l,defaultChecked:!0,storageKey:"vds-player::keyboard-animations",onChange(u){t.set(u)}})})})}function Xe(){return n(()=>{const{noAudioGain:e,translations:t}=r(),{audioTracks:a,canSetAudioGain:s}=v();return b(()=>!(s()&&!e())&&a().length<=1)()?null:i`
      <media-menu class="vds-audio-menu vds-menu">
        ${I({label:()=>S(t,"Audio"),icon:"menu-audio"})}
        <media-menu-items class="vds-menu-items">
          ${[Je(),ta()]}
        </media-menu-items>
      </media-menu>
    `})}function Je(){return n(()=>{const{translations:e}=r(),{audioTracks:t}=v(),a=d(e,"Default");return b(()=>t().length<=1)()?null:T({children:i`
        <media-menu class="vds-audio-tracks-menu vds-menu">
          ${I({label:()=>S(e,"Track")})}
          <media-menu-items class="vds-menu-items">
            <media-audio-radio-group
              class="vds-audio-track-radio-group vds-radio-group"
              empty-label=${a}
            >
              <template>
                <media-radio class="vds-audio-track-radio vds-radio">
                  <slot name="menu-radio-check-icon" data-class="vds-icon"></slot>
                  <span class="vds-radio-label" data-part="label"></span>
                </media-radio>
              </template>
            </media-audio-radio-group>
          </media-menu-items>
        </media-menu>
      `})})}function ta(){return n(()=>{const{noAudioGain:e,translations:t}=r(),{canSetAudioGain:a}=v();if(b(()=>!a()||e())())return null;const{audioGain:o}=v();return T({label:d(t,"Boost"),value:n(()=>Math.round(((o()??1)-1)*100)+"%"),children:[z({upIcon:"menu-audio-boost-up",downIcon:"menu-audio-boost-down",children:ea(),isMin:()=>((o()??1)-1)*100<=Lt(),isMax:()=>((o()??1)-1)*100===Bt()})]})})}function ea(){const{translations:e}=r(),t=d(e,"Boost"),a=Lt,s=Bt,o=aa;return i`
    <media-audio-gain-slider
      class="vds-audio-gain-slider vds-slider"
      aria-label=${t}
      min=${n(a)}
      max=${n(s)}
      step=${n(o)}
      key-step=${n(o)}
    >
      ${K()}${Q()}
    </media-audio-gain-slider>
  `}function Lt(){const{audioGains:e}=r(),t=e();return A(t)?t[0]??0:t.min}function Bt(){const{audioGains:e}=r(),t=e();return A(t)?t[t.length-1]??300:t.max}function aa(){const{audioGains:e}=r(),t=e();return A(t)?t[1]-t[0]||25:t.step}function sa(){return n(()=>{const{translations:e}=r(),{hasCaptions:t}=v(),a=d(e,"Off");return t()?i`
      <media-menu class="vds-captions-menu vds-menu">
        ${I({label:()=>S(e,"Captions"),icon:"menu-captions"})}
        <media-menu-items class="vds-menu-items">
          <media-captions-radio-group
            class="vds-captions-radio-group vds-radio-group"
            off-label=${a}
          >
            <template>
              <media-radio class="vds-caption-radio vds-radio">
                <slot name="menu-radio-check-icon" data-class="vds-icon"></slot>
                <span class="vds-radio-label" data-part="label"></span>
              </media-radio>
            </template>
          </media-captions-radio-group>
        </media-menu-items>
      </media-menu>
    `:null})}function na(){return n(()=>{const{translations:e}=r();return i`
      <media-menu class="vds-playback-menu vds-menu">
        ${I({label:()=>S(e,"Playback"),icon:"menu-playback"})}
        <media-menu-items class="vds-menu-items">
          ${[T({children:ia()}),oa(),da()]}
        </media-menu-items>
      </media-menu>
    `})}function ia(){const{remote:e}=k(),{translations:t}=r(),a="Loop";return N({label:d(t,a),children:q({label:a,storageKey:"vds-player::user-loop",onChange(s,o){e.userPrefersLoopChange(s,o)}})})}function oa(){return n(()=>{const{translations:e}=r(),{canSetPlaybackRate:t,playbackRate:a}=v();return t()?T({label:d(e,"Speed"),value:n(()=>a()===1?S(e,"Normal"):a()+"x"),children:[z({upIcon:"menu-speed-up",downIcon:"menu-speed-down",children:ra(),isMin:()=>a()===Gt(),isMax:()=>a()===Ft()})]}):null})}function Gt(){const{playbackRates:e}=r(),t=e();return A(t)?t[0]??0:t.min}function Ft(){const{playbackRates:e}=r(),t=e();return A(t)?t[t.length-1]??2:t.max}function la(){const{playbackRates:e}=r(),t=e();return A(t)?t[1]-t[0]||.25:t.step}function ra(){const{translations:e}=r(),t=d(e,"Speed"),a=Gt,s=Ft,o=la;return i`
    <media-speed-slider
      class="vds-speed-slider vds-slider"
      aria-label=${t}
      min=${n(a)}
      max=${n(s)}
      step=${n(o)}
      key-step=${n(o)}
    >
      ${K()}${Q()}
    </media-speed-slider>
  `}function ua(){const{remote:e,qualities:t}=k(),{autoQuality:a,canSetQuality:s,qualities:o}=v(),{translations:l}=r(),u="Auto";return b(()=>!s()||o().length<=1)()?null:N({label:d(l,u),children:q({label:u,checked:a,onChange(c,$){c?e.requestAutoQuality($):e.changeQuality(t.selectedIndex,$)}})})}function da(){return n(()=>{const{hideQualityBitrate:e,translations:t}=r(),{canSetQuality:a,qualities:s,quality:o}=v(),l=b(()=>!a()||s().length<=1),u=b(()=>pe(s()));return l()?null:T({label:d(t,"Quality"),value:n(()=>{const p=o()?.height,c=e()?null:o()?.bitrate,$=c&&c>0?`${(c/1e6).toFixed(2)} Mbps`:null,f=S(t,"Auto");return p?`${p}p${$?` (${$})`:""}`:f}),children:[z({upIcon:"menu-quality-up",downIcon:"menu-quality-down",children:ca(),isMin:()=>u()[0]===o(),isMax:()=>u().at(-1)===o()}),ua()]})})}function ca(){const{translations:e}=r(),t=d(e,"Quality");return i`
    <media-quality-slider class="vds-quality-slider vds-slider" aria-label=${t}>
      ${K()}${Q()}
    </media-quality-slider>
  `}function Vt({placement:e,portal:t,tooltip:a}){return n(()=>{const{viewType:s}=v(),{translations:o,menuPortal:l,noModal:u,menuGroup:p,smallWhen:c}=r(),$=b(()=>u()?W(e):c()?null:W(e)),f=b(()=>!c()&&p()==="bottom"&&s()==="video"?26:0),m=h(!1);Ae();function g(){m.set(!0)}function x(){m.set(!1)}const y=i`
      <media-menu-items
        class="vds-settings-menu-items vds-menu-items"
        placement=${n($)}
        offset=${n(f)}
      >
        ${n(()=>m()?[na(),je(),Xe(),sa()]:null)}
      </media-menu-items>
    `;return i`
      <media-menu class="vds-settings-menu vds-menu" @open=${g} @close=${x}>
        <media-tooltip class="vds-tooltip">
          <media-tooltip-trigger>
            <media-menu-button
              class="vds-menu-button vds-button"
              aria-label=${d(o,"Settings")}
            >
              ${w("menu-settings","vds-rotate-icon")}
            </media-menu-button>
          </media-tooltip-trigger>
          <media-tooltip-content
            class="vds-tooltip-content"
            placement=${H(a)?n(a):a}
          >
            ${d(o,"Settings")}
          </media-tooltip-content>
        </media-tooltip>
        ${t?At(l,y):y}
      </media-menu>
    `})}function ot({orientation:e,tooltip:t}){return n(()=>{const{pointer:a,muted:s,canSetVolume:o}=v();if(a()==="coarse"&&!s())return null;if(!o())return Tt({tooltip:t});const l=h(void 0),u=me(l);return i`
      <div class="vds-volume" ?data-active=${n(u)} ${R(l.set)}>
        ${Tt({tooltip:t})}
        <div class="vds-volume-popup">${pa({orientation:e})}</div>
      </div>
    `})}function pa({orientation:e}={}){const{translations:t}=r(),a=d(t,"Volume");return i`
    <media-volume-slider
      class="vds-volume-slider vds-slider"
      aria-label=${a}
      orientation=${re(e)}
    >
      <div class="vds-slider-track"></div>
      <div class="vds-slider-track-fill vds-slider-track"></div>
      <media-slider-preview class="vds-slider-preview" no-clamp>
        <media-slider-value class="vds-slider-value"></media-slider-value>
      </media-slider-preview>
      <div class="vds-slider-thumb"></div>
    </media-volume-slider>
  `}function lt(){const e=h(void 0),t=h(0),{thumbnails:a,translations:s,sliderChaptersMinWidth:o,disableTimeSlider:l,seekStep:u,noScrubGesture:p}=r(),c=d(s,"Seek"),$=n(l),f=n(()=>t()<o()),m=n(a);return bt(e,()=>{const g=e();g&&t.set(g.clientWidth)}),i`
    <media-time-slider
      class="vds-time-slider vds-slider"
      aria-label=${c}
      key-step=${n(u)}
      ?disabled=${$}
      ?no-swipe-gesture=${n(p)}
      ${R(e.set)}
    >
      <media-slider-chapters class="vds-slider-chapters" ?disabled=${f}>
        <template>
          <div class="vds-slider-chapter">
            <div class="vds-slider-track"></div>
            <div class="vds-slider-track-fill vds-slider-track"></div>
            <div class="vds-slider-progress vds-slider-track"></div>
          </div>
        </template>
      </media-slider-chapters>
      <div class="vds-slider-thumb"></div>
      <media-slider-preview class="vds-slider-preview">
        <media-slider-thumbnail
          class="vds-slider-thumbnail vds-thumbnail"
          .src=${m}
        ></media-slider-thumbnail>
        <div class="vds-slider-chapter-title" data-part="chapter-title"></div>
        <media-slider-value class="vds-slider-value"></media-slider-value>
      </media-slider-preview>
    </media-time-slider>
  `}function ma(){return i`
    <div class="vds-time-group">
      ${n(()=>{const{duration:e}=v();return e()?[i`<media-time class="vds-time" type="current"></media-time>`,i`<div class="vds-time-divider">/</div>`,i`<media-time class="vds-time" type="duration"></media-time>`]:null})}
    </div>
  `}function va(){return n(()=>{const{live:e,duration:t}=v();return e()?Mt():t()?i`<media-time class="vds-time" type="current" toggle remainder></media-time>`:null})}function Nt(){return n(()=>{const{live:e}=v();return e()?Mt():ma()})}function Rt(){return n(()=>{const{textTracks:e}=k(),{title:t,started:a}=v(),s=h(null);return yt(e,"chapters",s.set),s()&&(a()||!t())?Wt():i`<media-title class="vds-chapter-title"></media-title>`})}function Wt(){return i`<media-chapter-title class="vds-chapter-title"></media-chapter-title>`}function fa(){return[Z(),et(),i`
      <media-controls class="vds-controls">
        <media-controls-group class="vds-controls-group">
          ${[Dt({backward:!0,tooltip:"top start"}),_({tooltip:"top"}),Dt({tooltip:"top"}),$a(),lt(),va(),ot({orientation:"vertical",tooltip:"top"}),J({tooltip:"top"}),tt(),X({tooltip:"top"}),ba()]}
        </media-controls-group>
      </media-controls>
    `]}function $a(){return n(()=>{let e=h(void 0),t=h(!1),a=k(),{title:s,started:o,currentTime:l,ended:u}=v(),{translations:p}=r(),c=ve(e),$=()=>o()||l()>0;const f=()=>{const x=u()?"Replay":$()?"Continue":"Play";return`${S(p,x)}: ${s()}`};C(()=>{c()&&document.activeElement===document.body&&a.player.el?.focus({preventScroll:!0})});function m(){const x=e(),y=!!x&&!c()&&x.clientWidth<x.children[0].clientWidth;x&&ue(x,"vds-marquee",y),t.set(y)}function g(){return i`
        <span class="vds-title-text">
          ${n(f)}${n(()=>$()?Wt():null)}
        </span>
      `}return bt(e,m),s()?i`
          <span class="vds-title" title=${n(f)} ${R(e.set)}>
            ${[g(),n(()=>t()&&!c()?g():null)]}
          </span>
        `:D()})}function ba(){const e="top end";return[It({tooltip:"top",placement:e,portal:!0}),Vt({tooltip:"top end",placement:e,portal:!0})]}class _t extends be{async loadIcons(){const t=(await import("./vidstack-DQveMire.js")).icons,a={};for(const s of Object.keys(t))a[s]=ge({name:s,paths:t[s]});return a}}class ga extends vt(gt,Se){static tagName="media-audio-layout";static attrs={smallWhen:{converter(t){return t!=="never"&&!!t}}};#t;#e=h(!1);onSetup(){this.forwardKeepAlive=!1,this.#t=k(),this.classList.add("vds-audio-layout"),this.#n()}onConnect(){kt("audio",()=>this.isMatch),this.#s()}render(){return n(this.#a.bind(this))}#a(){return this.isMatch?fa():null}#s(){const{menuPortal:t}=r();C(()=>{if(!this.isMatch)return;const a=Ot(this,this.menuContainer,"vds-audio-layout",()=>this.isSmallLayout),s=a?[this,a]:[this];return(this.$props.customIcons()?new ht(s):new _t(s)).connect(),t.set(a),()=>{a.remove(),t.set(null)}})}#n(){const{pointer:t}=this.#t.$state;C(()=>{t()==="coarse"&&C(this.#i.bind(this))})}#i(){if(!this.#e()){j(this,"pointerdown",this.#o.bind(this),{capture:!0});return}j(this,"pointerdown",t=>t.stopPropagation()),j(window,"pointerdown",this.#l.bind(this))}#o(t){const{target:a}=t;fe(a)&&a.closest(".vds-time-slider")&&(t.stopImmediatePropagation(),this.setAttribute("data-scrubbing",""),this.#e.set(!0))}#l(){this.#e.set(!1),this.removeAttribute("data-scrubbing")}}class ha extends Y{static props={...super.props,when:({viewType:t})=>t==="video",smallWhen:({width:t,height:a})=>t<576||a<380}}function Et(){return n(()=>{const e=k(),{noKeyboardAnimations:t,userPrefersKeyboardAnimations:a}=r();if(b(()=>t()||!a())())return null;const o=h(!1),{lastKeyboardAction:l}=e.$state;C(()=>{o.set(!!l());const m=setTimeout(()=>o.set(!1),500);return()=>{o.set(!1),window.clearTimeout(m)}});const u=b(()=>{const m=l()?.action;return m&&o()?F(m):null}),p=b(()=>`vds-kb-action${o()?"":" hidden"}`),c=b(ya),$=b(()=>{const m=xa();return m?$e(m):null});function f(){const m=$();return m?i`
        <div class="vds-kb-bezel">
          <div class="vds-kb-icon">${m}</div>
        </div>
      `:null}return i`
      <div class=${n(p)} data-action=${n(u)}>
        <div class="vds-kb-text-wrapper">
          <div class="vds-kb-text">${n(c)}</div>
        </div>
        ${n(()=>de(l(),f()))}
      </div>
    `})}function ya(){const{$state:e}=k(),t=e.lastKeyboardAction()?.action,a=e.audioGain()??1;switch(t){case"toggleMuted":return e.muted()?"0%":Kt(e.volume(),a);case"volumeUp":case"volumeDown":return Kt(e.volume(),a);default:return""}}function Kt(e,t){return`${Math.round(e*t*100)}%`}function xa(){const{$state:e}=k();switch(e.lastKeyboardAction()?.action){case"togglePaused":return e.paused()?"kb-pause-icon":"kb-play-icon";case"toggleMuted":return e.muted()||e.volume()===0?"kb-mute-icon":e.volume()>=.5?"kb-volume-up-icon":"kb-volume-down-icon";case"toggleFullscreen":return`kb-fs-${e.fullscreen()?"enter":"exit"}-icon`;case"togglePictureInPicture":return`kb-pip-${e.pictureInPicture()?"enter":"exit"}-icon`;case"toggleCaptions":return e.hasCaptions()?`kb-cc-${e.textTrack()?"on":"off"}-icon`:null;case"volumeUp":return"kb-volume-up-icon";case"volumeDown":return"kb-volume-down-icon";case"seekForward":return"kb-seek-forward-icon";case"seekBackward":return"kb-seek-backward-icon";default:return null}}function Sa(){return[Z(),Qt(),U(),Et(),et(),i`<div class="vds-scrim"></div>`,i`
      <media-controls class="vds-controls">
        ${[wa(),D(),i`<media-controls-group class="vds-controls-group"></media-controls-group>`,D(),i`
            <media-controls-group class="vds-controls-group">
              ${lt()}
            </media-controls-group>
          `,i`
            <media-controls-group class="vds-controls-group">
              ${[_({tooltip:"top start"}),ot({orientation:"horizontal",tooltip:"top"}),Nt(),Rt(),J({tooltip:"top"}),ka(),X({tooltip:"top"}),wt({tooltip:"top"}),tt(),ke(),Ct({tooltip:"top end"})]}
            </media-controls-group>
          `]}
      </media-controls>
    `]}function ka(){return n(()=>{const{menuGroup:e}=r();return e()==="bottom"?rt():null})}function wa(){return i`
    <media-controls-group class="vds-controls-group">
      ${n(()=>{const{menuGroup:e}=r();return e()==="top"?[D(),rt()]:null})}
    </media-controls-group>
  `}function Ta(){return[Z(),Qt(),U(),et(),Et(),i`<div class="vds-scrim"></div>`,i`
      <media-controls class="vds-controls">
        <media-controls-group class="vds-controls-group">
          ${[X({tooltip:"top start"}),wt({tooltip:"bottom start"}),D(),J({tooltip:"bottom"}),tt(),rt(),ot({orientation:"vertical",tooltip:"bottom end"})]}
        </media-controls-group>

        ${D()}

        <media-controls-group class="vds-controls-group" style="pointer-events: none;">
          ${[D(),_({tooltip:"top"}),D()]}
        </media-controls-group>

        ${D()}

        <media-controls-group class="vds-controls-group">
          ${[Nt(),Rt(),Ct({tooltip:"top end"})]}
        </media-controls-group>

        <media-controls-group class="vds-controls-group">
          ${lt()}
        </media-controls-group>
      </media-controls>
    `,Da()]}function Ca(){return i`
    <div class="vds-load-container">
      ${[U(),_({tooltip:"top"})]}
    </div>
  `}function Da(){return n(()=>{const{duration:e}=v();return e()===0?null:i`
      <div class="vds-start-duration">
        <media-time class="vds-time" type="duration"></media-time>
      </div>
    `})}function U(){return i`
    <div class="vds-buffering-indicator">
      <media-spinner class="vds-buffering-spinner"></media-spinner>
    </div>
  `}function rt(){const{menuGroup:e,smallWhen:t}=r(),a=()=>e()==="top"||t()?"bottom":"top",s=b(()=>`${a()} ${e()==="top"?"end":"center"}`),o=b(()=>`${a()} end`);return[It({tooltip:s,placement:o,portal:!0}),Vt({tooltip:s,placement:o,portal:!0})]}function Qt(){return n(()=>{const{noGestures:e}=r();return e()?null:i`
      <div class="vds-gestures">
        <media-gesture class="vds-gesture" event="pointerup" action="toggle:paused"></media-gesture>
        <media-gesture
          class="vds-gesture"
          event="pointerup"
          action="toggle:controls"
        ></media-gesture>
        <media-gesture
          class="vds-gesture"
          event="dblpointerup"
          action="toggle:fullscreen"
        ></media-gesture>
        <media-gesture class="vds-gesture" event="dblpointerup" action="seek:-10"></media-gesture>
        <media-gesture class="vds-gesture" event="dblpointerup" action="seek:10"></media-gesture>
      </div>
    `})}class Ma extends vt(gt,ha){static tagName="media-video-layout";static attrs={smallWhen:{converter(t){return t!=="never"&&!!t}}};#t;onSetup(){this.forwardKeepAlive=!1,this.#t=k(),this.classList.add("vds-video-layout")}onConnect(){kt("video",()=>this.isMatch),this.#e()}render(){return n(this.#a.bind(this))}#e(){const{menuPortal:t}=r();C(()=>{if(!this.isMatch)return;const a=Ot(this,this.menuContainer,"vds-video-layout",()=>this.isSmallLayout),s=a?[this,a]:[this];return(this.$props.customIcons()?new ht(s):new _t(s)).connect(),t.set(a),()=>{a.remove(),t.set(null)}})}#a(){const{load:t}=this.#t.$props,{canLoad:a,streamType:s,nativeControls:o}=this.#t.$state;return!o()&&this.isMatch?t()==="play"&&!a()?Ca():s()==="unknown"?U():this.isSmallLayout?Ta():Sa():null}}ft(ga),ft(Ma);
