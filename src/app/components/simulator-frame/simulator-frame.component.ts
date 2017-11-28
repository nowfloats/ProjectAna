import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { SafeResourceUrl, DomSanitizer } from '@angular/platform-browser';

@Component({
	selector: 'app-simulator-frame',
	templateUrl: './simulator-frame.component.html',
	styleUrls: ['./simulator-frame.component.css']
})
export class SimulatorFrameComponent implements OnInit {

	constructor(private sanitizer: DomSanitizer) {
		let param = {
			debug: true,
			brandingConfig: {
				primaryBackgroundColor: '#8cc83c',
				primaryForegroundColor: 'white',
				secondaryBackgroundColor: '#3c3c3c',
				logoUrl: '/favicon.ico',
				agentName: "ANA Simulator",
				frameHeight: '70vh',
				frameWidth: '360px',
			}
		};
		let url = `simulator/index.html?sim=${btoa(JSON.stringify(param))}`;
		console.log(url);
		this.iframeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
	}

	ngOnInit() {

	}
	iframeUrl: SafeResourceUrl;
	isOpen: boolean = false;

	@ViewChild('anaRoot')
	anaRoot: ElementRef;

	@ViewChild('simulatorIFrame')
	simulatorIFrame: ElementRef;

	frame() {
		return (this.simulatorIFrame.nativeElement as HTMLIFrameElement).contentWindow;
	}

	minMaxBtnClick() {
		this.isOpen = !this.isOpen;
		if (this.anaRoot && this.anaRoot.nativeElement)
			(<HTMLDivElement>this.anaRoot.nativeElement).classList.remove('ana-hidden');
	};
}
