import { Component, OnInit, ElementRef } from '@angular/core';
import { HighlightJsService } from 'angular2-highlight-js';
import { AfterViewInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
import { DataService } from '../../../../services/data.service';
import { ActivatedRoute } from '@angular/router';

@Component({
	selector: 'app-deploy-landing',
	templateUrl: './deploy-landing.component.html',
	styleUrls: ['./deploy-landing.component.css']
})
export class DeployLandingComponent implements OnInit, AfterViewInit {

	constructor(
		private el: ElementRef,
		private highlight: HighlightJsService,
		private electron: ElectronService,
		private route: ActivatedRoute,
		private dataService: DataService) {
		this.route.queryParams.subscribe(x => {
			if (x && x['businessId']) {
				this.webOptions.businessId = x['businessId'];
			}
			if (x && x['chatFlowId']) {
				this.webOptions.flowId = x['chatFlowId'];
			}
		});
	}

	ngOnInit() {

	}

	webOptions: AnaChatWebOptions = {};

	ngAfterViewInit() {
		this.highlight.highlight(this.el.nativeElement.querySelector('.code'));
	}

	open(url: string) {
		this.electron.shell.openExternal(url);
	}

	webSnippet() {
		return `<script type="text/javascript" id="ana-web-chat-script"

src="${this.webOptions.websdkUrl}/assets/embed/ana-web-chat-plugin.js" 
ana-endpoint="${this.webOptions.webSocketsUrl}/wscustomers/chatcustomers-websocket"
ana-businessid="${this.webOptions.businessId}"
ana-primary-bg="${this.webOptions.accentColor}"
ana-flowid="${this.webOptions.flowId}"

ana-logo-url="${this.webOptions.logoUrl}"
ana-agent-name="${this.webOptions.title}"
ana-agent-desc="${this.webOptions.desc}"

ana-iframe-src="${this.webOptions.websdkUrl}"
ana-api-endpoint="${this.dataService.chatServer.ServerUrl}"
ana-gmaps-key="${this.webOptions.gmapsKey}"

ana-primary-fg="${this.webOptions.foregroundColor}"
ana-secondary-bg="${this.webOptions.secondaryColor}"
ana-frame-height="${this.webOptions.height}"
ana-frame-width="${this.webOptions.width}"

ana-fullpage="${this.webOptions.isFullPage}"
></script>`;
	}
}

export interface AnaChatWebOptions {
	accentColor?: string;
	logoUrl?: string;
	title?: string;
	desc?: string;
	isFullPage?: boolean;
	autoOpenSecs?: number;

	allowChatReset?: boolean;
	enableHtmlMessages?: boolean;
	showPoweredByAna?: boolean;
	gmapsKey?: string;
	foregroundColor?: string;
	secondaryColor?: string;
	height?: string;
	width?: string;

	websdkUrl?: string;
	webSocketsUrl?: string;

	businessId?: string;
	flowId?: string;
}
