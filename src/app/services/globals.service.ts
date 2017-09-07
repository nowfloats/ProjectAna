import { Injectable } from '@angular/core';
import * as ChatFlowModels from '../models/chatflow.models'
import { ChatFlowComponent } from '../components/chatflow/chatflow.component'

@Injectable()
export class GlobalsService {
    constructor() { }

    chatFlowComponent: ChatFlowComponent;
}
