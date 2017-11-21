import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalsService } from '../../services/globals.service';
import { ChatFlowPack } from '../../models/chatflow.models';
@Component({
	selector: 'app-startup',
	templateUrl: './startup.component.html',
	styleUrls: ['./startup.component.css']
})
export class StartupComponent implements OnInit {

	constructor(
		private router: Router,
		private globals: GlobalsService) {
	}

	ngOnInit() {

	}

	newChatBotProject() {
		alert('New');
	}

	openChatBotProject() {
		let fileInput = document.createElement('input');
		fileInput.type = 'file';
		fileInput.onchange = (event) => {
			if (fileInput.files && fileInput.files[0]) {
				let selectedFile = fileInput.files[0];
				if (selectedFile.name.endsWith('.anaproj')) {
					let reader = new FileReader();
					reader.onload = (evt) => {
						console.log("File Loaded: " + reader.result);
						this.globals.currentChatProject = JSON.parse(reader.result) as ChatFlowPack;
						this.router.navigateByUrl('/designer');
					};
					reader.onerror = () => {
						alert('Unable to load the file!');
					};
					reader.readAsText(selectedFile, "UTF-8");
				} else
					alert('Invalid file. Please select a valid Ana project file');
			}
		};
		fileInput.click();
	}
}
