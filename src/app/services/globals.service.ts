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

	uuidv4() {
		return (<any>[1e7] + -1e3 + -4e3 + -8e3 + -1e11).toString().replace(/[018]/g,
			c => (<any>c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> <any>c / 4).toString(16)
		)
	}
	getVariableType(x) {
		if (Array.isArray(x)) return VariableType.Array;
		else if (typeof x == 'string') return VariableType.String;
		else if (x != null && typeof x == 'object') return VariableType.Object;
		else return VariableType.Other;
	}
}

export enum VariableType {
	Array, String, Object, Other
}
