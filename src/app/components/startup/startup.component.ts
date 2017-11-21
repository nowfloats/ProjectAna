import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalsService } from '../../services/globals.service';
import { SettingsService } from '../../services/settings.service';
import * as models from '../../models/chatflow.models';
import { ObjectID } from 'bson';
@Component({
	selector: 'app-startup',
	templateUrl: './startup.component.html',
	styleUrls: ['./startup.component.css']
})
export class StartupComponent implements OnInit {

	constructor(
		private router: Router,
		private globals: GlobalsService,
		private settings: SettingsService) {
		this.globals.currentPageName = "Startup";
		this.loadSavedProjects();
	}

	loadSavedProjects() {
		this.savedProjects = this.settings.listSavedChatProjectNames();
	}

	savedProjects: string[] = [];

	ngOnInit() {

	}
	importProject() {
		let fileInput = document.createElement('input');
		fileInput.type = 'file';
		fileInput.onchange = (event) => {
			if (fileInput.files && fileInput.files[0]) {
				let selectedFile = fileInput.files[0];
				if (selectedFile.name.endsWith('.anaproj')) {
					let reader = new FileReader();
					reader.onload = (evt) => {
						let pack = JSON.parse(reader.result) as models.ChatFlowPack;
						let projName = selectedFile.name.replace(new RegExp('\.anaproj$'), '');
						this.settings.saveChatProject(projName, pack, false);
						this.openChatBotProject(projName);
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

	addProject() {
		let name = prompt('Enter a name for your new chat bot project');
		if (!name)
			return;

		let firstNode = {
			Name: 'New Node',
			Id: new ObjectID().toHexString(),
			Buttons: [],
			Sections: [],
			NodeType: models.NodeType.Combination,
			TimeoutInMs: 0
		};
		let _id = new ObjectID().toHexString();
		let defaultFlow: models.ChatFlowPack = {
			ChatNodes: [firstNode],
			CreatedOn: new Date(),
			UpdatedOn: new Date(),
			NodeLocations: {},
			ProjectId: _id,
			WebNodeLocations: {},
			_id: _id
		};
		defaultFlow.NodeLocations[firstNode.Id] = { X: 500, Y: 500 };
		defaultFlow.WebNodeLocations[firstNode.Id] = { X: 500, Y: 500 };
		this.settings.saveChatProject(name, defaultFlow, false);

		this.openChatBotProject(name);
	}

	isExpanded(proj: string) {
		return this.savedProjects.indexOf(proj) == this.savedProjects.length - 1;
	}

	openChatBotProject(name: string) {
		this.router.navigateByUrl('/designer?proj=' + encodeURIComponent(name));
	}
	renameChatBotProject(name: string) {
		let newName = prompt("Enter a new name: ", name);
		if (newName)
			this.settings.renameChatProject(name, newName);
	}
	deleteChatBotProject(name: string) {
		if (confirm(`Are you sure you want to delete '${name}'`)) {
			this.settings.deleteChatProject(name);
			this.loadSavedProjects();
		}
	}
}