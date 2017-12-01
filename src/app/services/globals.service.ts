import { Injectable } from '@angular/core';
import * as models from '../models/chatflow.models';
import { ANADate, ANATime, AddressInput } from '../models/ana-chat.models';
import { Title } from '@angular/platform-browser';
import { ChatFlowComponent } from '../components/chatflow/chatflow.component';
import * as moment from 'moment';
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

	anaDateDisplay(anaDate: ANADate) {
		return moment({
			year: parseInt(anaDate.year),
			month: parseInt(anaDate.month) - 1,
			day: parseInt(anaDate.mday)
		}).format("MM-DD-YYYY");
	}

	anaTimeDisplay(anaTime: ANATime) {
		return this.timeDisplay(`${anaTime.hour}:${anaTime.minute}:${anaTime.second}`);
	}

	anaAddressDisplay(anaAddress: AddressInput) {
		return [anaAddress.line1, anaAddress.area, anaAddress.city, anaAddress.state, anaAddress.country, anaAddress.pin].filter(x => x).join(", ");
	}

	timeDisplay(time: any) {
		// Check correct time format and split into components
		time = time.toString().match(/^([01]\d|2[0-3])(:)([0-5]\d)(:[0-5]\d)?$/) || [time];

		if (time.length > 1) { // If time format correct
			time = time.slice(1);  // Remove full string match value
			time[5] = +time[0] < 12 ? ' AM' : ' PM'; // Set AM/PM
			time[0] = +time[0] % 12 || 12; // Adjust hours
		}
		return time.join(''); // return adjusted time or original string
	}
}

export enum VariableType {
	Array, String, Object, Other
}
