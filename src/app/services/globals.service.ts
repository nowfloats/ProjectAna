import { Injectable } from '@angular/core';
import * as models from '../models/chatflow.models';
import { Title } from '@angular/platform-browser';
import { ChatFlowComponent } from '../components/chatflow/chatflow.component';

@Injectable()
export class GlobalsService {
	constructor(private title: Title) { }
	appName = 'Ana Conversation Studio';

	chatFlowComponent: ChatFlowComponent;
	loading: boolean = false;

	setPageTitle(title?: string) {
		if (title)
			this.title.setTitle(`${title} - ${this.appName}`);
		else
			this.title.setTitle(this.appName);
	}

	downloadTextAsFile(filename, text) {
		var element = document.createElement('a');
		element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
		element.setAttribute('download', filename);

		element.style.display = 'none';
		document.body.appendChild(element);

		element.click();

		document.body.removeChild(element);
	}
}
