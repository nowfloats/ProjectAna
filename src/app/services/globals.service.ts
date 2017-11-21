import { Injectable } from '@angular/core';
import * as models from '../models/chatflow.models'
import { ChatFlowComponent } from '../components/chatflow/chatflow.component'


@Injectable()
export class GlobalsService {
	constructor() { }

	chatFlowComponent: ChatFlowComponent;
	loading: boolean = false;
	currentPageName: string = '';
	currentChatProject: models.ChatFlowPack;
}
