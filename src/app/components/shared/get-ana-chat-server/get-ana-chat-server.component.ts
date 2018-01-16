import { Component, OnInit } from '@angular/core';
import { ElectronService } from 'ngx-electron';
@Component({
	selector: 'app-get-ana-chat-server',
	templateUrl: './get-ana-chat-server.component.html',
	styleUrls: ['./get-ana-chat-server.component.css']
})
export class GetAnaChatServerComponent implements OnInit {

	constructor(private electron: ElectronService) { }

	ngOnInit() {
	}

	selfHost() {
		this.electron.shell.openExternal('https://www.ana.chat/self-hosting.html?r=' + Math.random());
	}

	anaCloud() {
		let win = new this.electron.remote.BrowserWindow({
			width: 360,
			height: 700,
			center: true,
			resizable: false
		});
		win.on('closed', () => {
			win = null
		});
		win.loadURL('https://with.ana.chat/ana-cloud-signup/');
	}
}
