import { Component, OnInit } from '@angular/core';
import { faBook } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-favicon',
  standalone: true,
  template: `<div style="display: none;"></div>`
})
export class FaviconComponent implements OnInit {
  
  ngOnInit() {
    this.setFaviconFromFontAwesome();
  }

  private setFaviconFromFontAwesome() {
    // Create SVG from FontAwesome fa-book icon
    const svgContent = `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" width="32" height="32">
        <path fill="#4a90e2" d="${faBook.icon[4]}"/>
      </svg>
    `;
    
    // Create blob from SVG
    const blob = new Blob([svgContent], { type: 'image/svg+xml' });
    const url = URL.createObjectURL(blob);
    
    // Set favicon
    const existingLink = document.querySelector("link[rel*='icon']") as HTMLLinkElement;
    if (existingLink) {
      existingLink.href = url;
    } else {
      const link = document.createElement('link') as HTMLLinkElement;
      link.type = 'image/svg+xml';
      link.rel = 'shortcut icon';
      link.href = url;
      document.getElementsByTagName('head')[0].appendChild(link);
    }
  }
} 