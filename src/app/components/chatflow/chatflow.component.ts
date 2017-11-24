import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, HostListener } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { MdDialog, MdDialogRef, MD_DIALOG_DATA, MdSnackBar } from '@angular/material';
import { ChatFlowService } from '../../services/chatflow.service'
import { SettingsService } from '../../services/settings.service'
import { GlobalsService } from '../../services/globals.service'
import * as models from '../../models/chatflow.models';
import { NodeEditorComponent } from '../nodeeditor/nodeeditor.component';
import { PublishDialogComponent } from '../publish-dialog/publish-dialog.component';
import { ChatServerManagerComponent } from '../chat-server-manager/chat-server-manager.component';
import { MdMenuTrigger } from '@angular/material';

import { ObjectID } from 'bson';
import { InfoDialogService } from '../../services/info-dialog.service';

@Component({
	selector: 'app-chatflow',
	templateUrl: './chatflow.component.html',
	styleUrls: ['./chatflow.component.css'],
})
export class ChatFlowComponent implements OnInit {

	constructor(
		private chatFlowService: ChatFlowService,
		public dialog: MdDialog,
		public infoDialog: InfoDialogService,
		public route: ActivatedRoute,
		public router: Router,
		public snakbar: MdSnackBar,
		public globalsService: GlobalsService,
		public settings: SettingsService) {

		this.chatFlowNetwork = new ChatFlowNetwork(this);
		this.chatFlowNetwork.newChatNodeConnection.isHidden = true;
		this._viewBoxX = 0;
		this._viewBoxY = 0;
		this._viewBoxWidth = Config.defaultDesignerWidth;
		this._viewBoxHeight = Config.defaultDesignerHeight;

		globalsService.chatFlowComponent = this;

		this.MH = new models.ModelHelpers(globalsService, infoDialog);
	}
	chatFlowNetwork: ChatFlowNetwork;
	MH: models.ModelHelpers;

	@ViewChild('chatflowRoot')
	chatflowRoot: ElementRef;
	projName: string = "";
	ngOnInit(): void {
		this.route.queryParamMap.subscribe(x => {
			this.projName = x.get('proj');
			if (this.projName) {
				let proj = this.settings.getChatProject(this.projName);
				if (proj)
					this.loadChatFlowPack(proj);
				else
					this.router.navigateByUrl('/startup');
			}
		});
	}

	chatFlowRootSVG() {
		return this.chatflowRoot.nativeElement as SVGSVGElement;
	}

	updateLayout() {
		if (this.chatFlowNetwork &&
			this.chatFlowNetwork.chatNodeVMs &&
			this.chatFlowNetwork.chatNodeVMs.length > 0 &&
			this.chatflowRoot) {
			let ele = this.chatFlowRootSVG();
			if (ele.querySelector) { //Initialization issues, proceed only if querySelector is available.
				for (let i = 0; i < this.chatFlowNetwork.chatNodeVMs.length; i++) {
					let chatNode = this.chatFlowNetwork.chatNodeVMs[i];

					let _updateNodeLayoutInit = this.updateNodeLayout(chatNode);
					if (!_updateNodeLayoutInit || !chatNode._layoutUpdated) {
						window.requestAnimationFrame(() => this.updateLayout());
						break;
					}
				}
			}
		}
	}

	updateNodeLayout(chatNodeVM: ChatNodeVM): boolean {
		let btnsTable = this.chatFlowRootSVG().querySelector(`table[node-id='${chatNodeVM.chatNode.Id}']`) as HTMLTableElement;
		if (btnsTable) {

			if (!chatNodeVM._layoutUpdated) //If this is not done, when new section is added to the node, node's width is also increasing abnormally!
				chatNodeVM._btnTableWidth = btnsTable.getBoundingClientRect().width;
			else
				chatNodeVM._btnTableWidth = btnsTable.clientWidth;

			chatNodeVM._width = ((chatNodeVM._width > chatNodeVM._btnTableWidth) ? chatNodeVM._width : chatNodeVM._btnTableWidth);

			window.requestAnimationFrame(() => {
				let nodeRoot = this.chatFlowRootSVG().querySelector(`div[node-id='${chatNodeVM.chatNode.Id}']`) as HTMLDivElement;
				chatNodeVM._height = nodeRoot.clientHeight;
				chatNodeVM._layoutUpdated = true;
			});
			return true;
		}

		return false;
	}

	ngTr(x: number, y: number) {
		return `translate(${x},${y})`;
	}

	mouseMove(event: MouseEvent) {
		if (!this.chatFlowNetwork.newChatNodeConnection.isHidden) {
			let targetXY = this.transformCoordinates(event.pageX, event.pageY, event);
			this.chatFlowNetwork.newChatNodeConnection.destX = targetXY.x - 30;
			this.chatFlowNetwork.newChatNodeConnection.destY = targetXY.y - 30;
		}

		if (this.chatFlowNetwork.draggingChatNode) {
			try {
				let targetXY = this.transformCoordinates(event.pageX, event.pageY, event);
				let offset = this.chatFlowNetwork.draggingChatNodeOffset;
				this.chatFlowNetwork.draggingChatNode._x = targetXY.x - offset.x;
				this.chatFlowNetwork.draggingChatNode._y = targetXY.y - offset.y;
			} catch (e) {
				this.chatFlowNetwork.draggingChatNode._x += event.movementX;
				this.chatFlowNetwork.draggingChatNode._y += event.movementY;
			}
		}

		if (this._isMouseDown) {
			this._viewBoxX -= event.movementX;
			this._viewBoxY -= event.movementY;
		}
	}

	transformCoordinates(x: number, y: number, event: MouseEvent) {
		let svg_elem = this.chatFlowRootSVG();
		let matrix = svg_elem.getScreenCTM();
		let point = svg_elem.createSVGPoint();
		point.x = x - event.view.pageXOffset;
		point.y = y - event.view.pageYOffset;
		return point.matrixTransform(matrix.inverse());
	}

	_isMouseDown = false;
	mouseDown(event: MouseEvent) {

		//cancel any ongoing animation as user might have interrupted it by doing the mouse down.
		this.zoomCancel();

		//Check if mouse is captured by others
		if (this.chatFlowNetwork.newChatNodeConnection.isHidden && !this.chatFlowNetwork.draggingChatNode)
			this._isMouseDown = true;
		else
			this._isMouseDown = false;
	}

	mouseUp(event: MouseEvent) {
		this.resetDraggingState();
	}

	mouseLeave(event: MouseEvent) {
		this.resetDraggingState();
	}

	_viewBoxWidth: number;
	_viewBoxHeight: number;

	_viewBoxX: number;
	_viewBoxY: number;
	viewBox() {
		//0 0 1000 500
		return `${this._viewBoxX} ${this._viewBoxY} ${this._viewBoxWidth} ${this._viewBoxHeight}`;
	}

	zoomToRect(x: number, y: number, width: number, height: number) {
		this._viewBoxX = x;
		this._viewBoxY = y;
		this._viewBoxWidth = width;
		this._viewBoxHeight = height;
	}

	animationFrameId: number = 0;
	zoomToRectWithAnimation(x: number, y: number, width: number, height: number) {
		this.zoomToRectAnimIntermediate(
			this._viewBoxX, this._viewBoxY, this._viewBoxWidth, this._viewBoxHeight,
			x, y, width, height);
	}
	zoomCancel() {
		if (this.animationFrameId)
			cancelAnimationFrame(this.animationFrameId);
	}
	zoomToRectAnimIntermediate(
		x1: number, y1: number, width1: number, height1: number,
		x2: number, y2: number, width2: number, height2: number) {

		let step = Config.viewBoxAnimStep * ((Math.abs(x1 - x2) + Math.abs(y1 - y2) + Math.abs(width1 - width2) + Math.abs(height1 - height2)) / 100);

		this._viewBoxX = this.tendValue(x1, x2, step);
		this._viewBoxY = this.tendValue(y1, y2, step);
		this._viewBoxWidth = this.tendValue(width1, width2, step);
		this._viewBoxHeight = this.tendValue(height1, height2, step);

		if (!this.approxEquals(this._viewBoxX, x2, step) ||
			!this.approxEquals(this._viewBoxY, y2, step) ||
			!this.approxEquals(this._viewBoxWidth, width2, step) ||
			!this.approxEquals(this._viewBoxHeight, height2, step))
			this.animationFrameId = requestAnimationFrame(() => {
				this.zoomToRectAnimIntermediate(
					this._viewBoxX, this._viewBoxY, this._viewBoxWidth, this._viewBoxHeight,
					x2, y2, width2, height2);
			});
		else
			this.animationFrameId = 0;
	}

	tendValue(value: number, tendsTo: number, step: number) {
		return (Math.abs(value - tendsTo) > step ? (value > tendsTo ? value - step : value + step) : value);
	}
	approxEquals(a: number, b: number, approx: number): boolean {
		return Math.abs(Math.round(b) - Math.round(a)) <= Math.round(approx);
	}

	fitViewToAllNodes() {
		this.fitViewToNodes(this.chatFlowNetwork.chatNodeVMs);
	}

	fitViewToNodes(chatNodeVMs: ChatNodeVM[]) {
		var Xs = chatNodeVMs.map(x => x._x);
		var Ys = chatNodeVMs.map(x => x._y);

		var XsWithWidth = chatNodeVMs.map(x => x._x + x._width);
		var YsWithHeight = chatNodeVMs.map(x => x._y + x._height);

		var minX = Math.min(...Xs);
		var minY = Math.min(...Ys);
		var maxX = Math.max(...XsWithWidth);
		var maxY = Math.max(...YsWithHeight);
		var width = maxX - minX;
		var height = maxY - minY;
		console.log('fitViewToNodes: ');
		console.log(width);
		console.log(height);
		this.zoomToRectWithAnimation(minX, minY, width, height);
	}

	designerWheel(event: WheelEvent) {
		event.preventDefault();

		//cancel any ongoing animation as user might have interrupted it by doing the mouse down.
		this.zoomCancel();

		let change = Config.zoomCoefficient * event.wheelDelta;
		if (this._viewBoxWidth - change > 0)
			this._viewBoxWidth -= change;

		if (this._viewBoxHeight - change > 0)
			this._viewBoxHeight -= change;
	}

	openEditor(chatNodeVM: ChatNodeVM) {
		let dialogRef = this.dialog.open(NodeEditorComponent, {
			width: '80%',
			data: chatNodeVM.chatNode
		});
	}

	addNewNode() {
		var newNodeVM = new ChatNodeVM(this.chatFlowNetwork, {
			Name: 'New Node',
			Id: new ObjectID().toHexString(),
			Buttons: [],
			Sections: [],
			NodeType: models.NodeType.Combination,
			TimeoutInMs: 0
		});
		newNodeVM._x = (this._viewBoxWidth / 2) + (Math.random() * 50);
		newNodeVM._y = (this._viewBoxHeight / 2) + (Math.random() * 50);
		newNodeVM._layoutUpdated = true; //To skip the loading indicator

		this.chatFlowNetwork.updateChatNodeConnections();
		this.updateLayout();
	}

	private resetDraggingState() {
		if (!this.chatFlowNetwork.newChatNodeConnection.isHidden)
			this.chatFlowNetwork.newChatNodeConnection.isHidden = true;
		if (this.chatFlowNetwork.draggingChatNode)
			delete this.chatFlowNetwork.draggingChatNode;
		this._isMouseDown = false;
	}
	private loadChatFlowPack(pack: models.ChatFlowPack) {

		if (pack.ChatNodes) {
			this.chatFlowNetwork.chatFlowPack = pack;
			this.chatFlowNetwork.chatNodeVMs = [];

			pack.ChatNodes.forEach(cn => {
				new ChatNodeVM(this.chatFlowNetwork, cn);

				cn.Buttons.forEach(btn => {
					btn.AdvancedOptions = ((btn.VariableValue || btn.ConditionMatchKey || btn.ConditionMatchValue || btn.ConditionOperator) ? true : false);
				});
			});

			this.chatFlowNetwork.chatNodeVMs.forEach(vm => {
				var locs = pack.NodeLocations;
				if (locs) {
					var loc = locs[vm.chatNode.Id];
					vm._x = loc.X;
					vm._y = loc.Y;
				}
			});

			this.chatFlowNetwork.updateChatNodeConnections();
			this.updateLayout();

			this.initialZoom();
		}
	}

	layoutReady() {
		if (!this.chatFlowNetwork.chatNodeVMs)
			return true;
		else
			return (this.chatFlowNetwork.chatNodeVMs.filter(x => x._layoutUpdated).length == this.chatFlowNetwork.chatNodeVMs.length);
	}

	initialZoom() {
		if (this.layoutReady())
			this.fitViewToAllNodes();
		else
			setTimeout(() => this.initialZoom(), 500);
	}

	saveChatFlow() {
		var nodeLocs: models.NodeLocations = {};

		for (let i = 0; i < this.chatFlowNetwork.chatNodeVMs.length; i++) {
			let item = this.chatFlowNetwork.chatNodeVMs[i];

			nodeLocs[item.chatNode.Id] = {
				X: item._x,
				Y: item._y
			};
		}

		let pack: models.ChatFlowPack = {
			ProjectId: this.chatFlowNetwork.chatFlowPack.ProjectId,
			ChatNodes: this.chatFlowNetwork.chatNodeVMs.map(x => x.chatNode),
			NodeLocations: nodeLocs,
			_id: this.chatFlowNetwork.chatFlowPack._id,
			CreatedOn: this.chatFlowNetwork.chatFlowPack.CreatedOn,
			UpdatedOn: this.chatFlowNetwork.chatFlowPack.UpdatedOn
		};
		this.settings.saveChatProject(this.projName, pack, true);
		this.snakbar.open('Chatbot project saved', 'Dismiss', {
			duration: 2000
		});
		return pack;
	}

	exportChatFlow() {
		let pack = this.saveChatFlow();
		this.globalsService.downloadTextAsFile(this.projName + ".anaproj", JSON.stringify(pack));
	}

	playChatFlow() {
		this.infoDialog.alert('Alert', 'Coming soon');
	}

	openPublishDialog() {
		this.dialog.open(PublishDialogComponent, {
			width: '60%',
			data: this.saveChatFlow()
		});
	}

	gotoStartup() {
		this.infoDialog.confirm('Save?', 'Do you want to save any unsaved changes before you close?', (ok) => {
			if (ok)
				this.saveChatFlow();
			this.router.navigateByUrl('/startup');
		});
	}
}

class ChatFlowNetwork {
	constructor(public parent: ChatFlowComponent) {
	}

	updateChatNodeConnections(): void {
		this.chatNodeConnections = [];

		this.chatNodeVMs.forEach(chatNodeVM => {
			chatNodeVM.chatNode.Buttons.forEach(srcBtn => {
				if (srcBtn.NextNodeId != null || srcBtn.NextNodeId != "") {
					let destNode = this.chatNodeVMs.filter(x => x.chatNode.Id == srcBtn.NextNodeId);
					if (destNode && destNode.length > 0)
						this.chatNodeConnections.push(new ChatNodeConnection(new ChatButtonConnector(chatNodeVM, srcBtn), destNode[0]));
				}
			});
		});
	}

	chatNodeConnections: ChatNodeConnection[] = [];
	chatNodeVMs: ChatNodeVM[] = [];
	chatFlowPack: models.ChatFlowPack;

	newChatNodeConnection: ChatNodeNewConnection = new ChatNodeNewConnection();
	draggingChatNode: ChatNodeVM;
	draggingChatNodeOffset: Point;
}

class ChatNodeConnection {
	constructor(
		public srcButtonConnector: ChatButtonConnector,
		public destChatNodeVM: ChatNodeVM) {
	}

	srcConnectorX() {
		return this.srcButtonConnector.x() - (this.pathWidth / 2);
	}
	srcConnectorY() {
		return this.srcButtonConnector.y();
	}

	destConnectorX() {
		return this.destChatNodeVM.nodeConnectorX() - (this.pathWidth / 2)
	}
	destConnectorY() {
		return this.destChatNodeVM.nodeConnectorY();
	}

	calcTangentOffset(pt1X: number, pt2X: number) {
		return ((pt2X - pt1X) / 2);
	}

	calcSrcTangentX() {
		let pt1X = this.srcConnectorX();
		let pt2X = this.destConnectorX();
		return pt1X + this.calcTangentOffset(pt1X, pt2X);
	}
	calcSrcTangentY() {
		return this.srcConnectorY();
	}

	calcDestTangentX() {
		let pt1X = this.srcConnectorX();
		let pt2X = this.destConnectorX();
		return pt2X - this.calcTangentOffset(pt1X, pt2X);
	}
	calcDestTangentY() {
		return this.destConnectorY();
	}

	path() {
		return `M${this.srcConnectorX()},${this.srcConnectorY()} 
                C${this.calcSrcTangentX()},${this.calcSrcTangentY()} 
                  ${this.calcDestTangentX()},${this.calcDestTangentY()} 
                ${this.destConnectorX()},${this.destConnectorY()}`;
	}

	closeButtonVisible = false;
	closeButtonPointX: number = 0;
	closeButtonPointY: number = 0;
	mouseEnter(event: MouseEvent) {
		let xy = this.destChatNodeVM.network.parent.transformCoordinates(event.pageX, event.pageY, event);
		this.closeButtonPointX = xy.x; //some offset from the cursor
		this.closeButtonPointY = xy.y; //some offset from the cursor
		this.closeButtonVisible = true;
		//alert(`${this.closeButtonPoint.x},${this.closeButtonPoint.y}`);
		setTimeout(() => {
			this.closeButtonVisible = false;
		}, 50000); //Hide the close button after 5secs
	}

	circleRadius = Config.buttonCircleRadius;
	pathWidth = Config.connectionPathWidth;
}

class ChatNodeNewConnection {
	srcButtonConnector: ChatButtonConnector;
	destX: number;
	destY: number;

	isHidden = false;
	canConnect = false;

	srcConnectorX() {
		if (this.srcButtonConnector)
			return this.srcButtonConnector.x();
		return 0;
	}
	srcConnectorY() {
		if (this.srcButtonConnector)
			return this.srcButtonConnector.y();
		return 0;
	}

	calcTangentOffset(pt1X: number, pt2X: number) {
		return ((pt2X - pt1X) / 2);
	}

	calcSrcTangentX() {
		let pt1X = this.srcConnectorX();
		let pt2X = this.destX;
		return pt1X + this.calcTangentOffset(pt1X, pt2X);
	}
	calcSrcTangentY() {
		return this.srcConnectorY();
	}

	calcDestTangentX() {
		let pt1X = this.srcConnectorX();
		let pt2X = this.destX;
		return pt2X - this.calcTangentOffset(pt1X, pt2X);
	}
	calcDestTangentY() {
		return this.destY;
	}

	path() {
		if (this.isHidden)
			return "M 0,0";

		return `M${this.srcConnectorX()},${this.srcConnectorY()} 
                C${this.calcSrcTangentX()},${this.calcSrcTangentY()} 
                  ${this.calcDestTangentX()},${this.calcDestTangentY()} 
                ${this.destX},${this.destY}`;
	}

	circleRadius = Config.buttonCircleRadius;
}

class ChatButtonConnector {
	constructor(
		public chatNodeVM: ChatNodeVM,
		public button: models.Button) {
	}

	x() {
		let btns = this.chatNodeVM.chatNode.Buttons;
		let btnsCount = btns.length;
		let eachPart = (this.chatNodeVM.width() / btnsCount);
		let _x = (
			this.chatNodeVM.x()
			+ ((eachPart) * (this.btnIndex() + 1))
			- eachPart / 2
			- this.chatNodeVM.circleRadius
		);
		return _x;
	}

	y() {
		var _y = this.chatNodeVM.y() + this.chatNodeVM.height();
		//console.log("YY- " + this.chatNodeVM.chatNode.Name + ": " + this.chatNodeVM.height());
		return _y;
	}

	circleRadius = Config.buttonCircleRadius;

	mouseDown(event: MouseEvent) {
		let nw = this.chatNodeVM.network;
		if (nw.newChatNodeConnection.isHidden)
			nw.newChatNodeConnection.isHidden = false;
		nw.newChatNodeConnection.srcButtonConnector = this;
		nw.newChatNodeConnection.destX = this.x();
		nw.newChatNodeConnection.destY = this.y();
	}

	btnIndex() {
		let btns = this.chatNodeVM.chatNode.Buttons;
		return btns.indexOf(this.button);
	}

	setButtonNextNodeId(nextNodeId: string) {
		this.button.NextNodeId = nextNodeId;
		this.chatNodeVM.network.updateChatNodeConnections();
	}

	isConnected() {
		return this.button.NextNodeId && (this.chatNodeVM.network.chatNodeVMs.filter(x => x.chatNode.Id == this.button.NextNodeId).length > 0);
	}
}

export class ChatNodeVM {
	constructor(
		public network: ChatFlowNetwork,
		public chatNode: models.ChatNode) {
		this.network.chatNodeVMs.push(this);

		this._x = (this.network.chatNodeVMs.indexOf(this)) * Config.defaultNodeWidth;
	}
	_layoutUpdated: boolean = false;
	_x: number = 0;
	_y: number = 0;

	_btnTableWidth: number = Config.defaultNodeWidth;
	_width: number = Config.defaultNodeWidth;
	width() {
		return this._width;
	}

	_height: number = Config.defaultNodeHeight;
	height() {
		return this._height;
	}

	x() {
		return this._x;
	}

	y() {
		return this._y;
	}

	cornerRadius: number = Config.defaultNodeCornerRadius;
	headerHeight: number = Config.defaultNodeHeaderHeight;

	mouseDown(event: MouseEvent) {
		this.network.draggingChatNode = this;

		let targetXY = this.network.parent.transformCoordinates(event.pageX, event.pageY, event);
		let mouseOffsetX = targetXY.x - this._x;
		let mouseOffsetY = targetXY.y - this._y;
		this.network.draggingChatNodeOffset = new Point(mouseOffsetX, mouseOffsetY);
	}

	mouseUp(event: MouseEvent) {
		let nw = this.network;
		if (!nw.newChatNodeConnection.isHidden) {
			if (nw.newChatNodeConnection.srcButtonConnector.chatNodeVM != this)
				nw.newChatNodeConnection.srcButtonConnector.setButtonNextNodeId(this.chatNode.Id);
		}
	}

	mouseEnter(event: MouseEvent) {
		let nw = this.network;
		if (!nw.newChatNodeConnection.isHidden) {
			if (nw.newChatNodeConnection.srcButtonConnector.chatNodeVM != this)
				nw.newChatNodeConnection.canConnect = true;
		}
	}

	mouseLeave(event: MouseEvent) {
		let nw = this.network;
		if (!nw.newChatNodeConnection.isHidden) {
			nw.newChatNodeConnection.canConnect = false;
		}
	}

	chatButtonConnectors() {
		return this.chatNode.Buttons.map(btn => new ChatButtonConnector(this, btn));
	}

	nodeConnectorY() {
		return this.y();
	}

	nodeConnectorX() {
		return (this.x()) + (this.width() / 2) - this.circleRadius;
	}

	circleRadius = Config.buttonCircleRadius;
}

class Point {
	constructor(public x: number, public y: number) { }
}

class Config {
	static defaultNodeWidth = 300;
	static defaultNodeHeight = 200;
	static defaultNodeHeaderHeight = 30;

	static defaultNodeCornerRadius = 20;

	static defaultSectionWidth = 30;
	static defaultSectionHeight = 30;

	static defaultDesignerWidth = 1366;
	static defaultDesignerHeight = 700;

	static buttonCircleRadius = 8;
	static designerMargin = 40;
	static connectionPathWidth = 3;

	static zoomCoefficient = 0.3;

	static viewBoxAnimStep = 10;
}
