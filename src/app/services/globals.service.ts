import { Injectable } from '@angular/core';
import * as models from '../models/chatflow.models'
import { ChatFlowComponent } from '../components/chatflow/chatflow.component'


@Injectable()
export class GlobalsService {
	constructor() { }

	chatFlowComponent: ChatFlowComponent;
	loading: boolean = false;
	currentPageName: string = '';

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
