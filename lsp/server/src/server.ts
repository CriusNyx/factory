/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
import {
	CompletionItem,
	CompletionItemKind,
	createConnection,
	Diagnostic,
	DiagnosticSeverity,
	DidChangeConfigurationNotification,
	type DocumentDiagnosticReport,
	DocumentDiagnosticReportKind,
	InitializeParams,
	InitializeResult,
	ProposedFeatures,
	TextDocumentPositionParams,
	TextDocuments,
	TextDocumentSyncKind,
} from "vscode-languageserver/node";

import { Position, TextDocument } from "vscode-languageserver-textdocument";
import * as dotnet from "node-api-dotnet";
import { Factory } from "node-api-dotnet";

dotnet.load(__dirname + "/bin/factory.dll");

const FactorySemanticType = Factory.Parsing.FactorySemanticType;
type FactorySemanticType = Factory.Parsing.FactorySemanticType;

const FactoryAnalytics = Factory.FactoryAnalytics;

const FactoryParser = Factory.Parsing.FactoryParser;

const FactoryLexon = Factory.Parsing.FactoryLexon;
type FactoryLexon = Factory.Parsing.FactoryLexon;

console.log({
	programHeads: FactoryParser.ProgramLexonHeads.map((x) => FactoryLexon[x]),
	recipeHeads: FactoryParser.RecipeExpLexonHeads.map((x) => FactoryLexon[x]),
});

// Create a connection for the server, using Node's IPC as a transport.
// Also include all preview / proposed LSP features.
const connection = createConnection(ProposedFeatures.all);

const tokenTypes = FactoryAnalytics.GetSemanticTypes();
const tokenModifiers = FactoryAnalytics.GetSemanticModifiers();

// Create a simple text document manager.
const documents = new TextDocuments(TextDocument);

let hasConfigurationCapability = false;
let hasWorkspaceFolderCapability = false;
let hasDiagnosticRelatedInformationCapability = false;

connection.onInitialize((params: InitializeParams) => {
	const capabilities = params.capabilities;

	// Does the client support the `workspace/configuration` request?
	// If not, we fall back using global settings.
	hasConfigurationCapability = !!(
		capabilities.workspace && !!capabilities.workspace.configuration
	);
	hasWorkspaceFolderCapability = !!(
		capabilities.workspace && !!capabilities.workspace.workspaceFolders
	);
	hasDiagnosticRelatedInformationCapability = !!(
		capabilities.textDocument &&
		capabilities.textDocument.publishDiagnostics &&
		capabilities.textDocument.publishDiagnostics.relatedInformation
	);

	const result: InitializeResult = {
		capabilities: {
			textDocumentSync: TextDocumentSyncKind.Incremental,
			// Tell the client that this server supports code completion.
			completionProvider: {
				resolveProvider: true,
			},
			diagnosticProvider: {
				interFileDependencies: false,
				workspaceDiagnostics: false,
			},
			semanticTokensProvider: {
				legend: {
					// Cast because typescript enums have weird encoding.
					tokenTypes,
					tokenModifiers,
				},
				full: true,
				range: false,
			},
		},
	};
	if (hasWorkspaceFolderCapability) {
		result.capabilities.workspace = {
			workspaceFolders: {
				supported: true,
			},
		};
	}
	return result;
});

connection.onInitialized(() => {
	if (hasConfigurationCapability) {
		// Register for all configuration changes.
		connection.client.register(
			DidChangeConfigurationNotification.type,
			undefined,
		);
	}
	if (hasWorkspaceFolderCapability) {
		connection.workspace.onDidChangeWorkspaceFolders((_event) => {
			connection.console.log("Workspace folder change event received.");
		});
	}
});

// The example settings
interface ExampleSettings {
	maxNumberOfProblems: number;
}

// The global settings, used when the `workspace/configuration` request is not supported by the client.
// Please note that this is not the case when using this server with the client provided in this example
// but could happen with other clients.
const defaultSettings: ExampleSettings = { maxNumberOfProblems: 1000 };
let globalSettings: ExampleSettings = defaultSettings;

// Cache the settings of all open documents
const documentSettings = new Map<string, Thenable<ExampleSettings>>();

connection.onDidChangeConfiguration((change) => {
	if (hasConfigurationCapability) {
		// Reset all cached document settings
		documentSettings.clear();
	} else {
		globalSettings = change.settings.languageServerExample || defaultSettings;
	}
	// Refresh the diagnostics since the `maxNumberOfProblems` could have changed.
	// We could optimize things here and re-fetch the setting first can compare it
	// to the existing setting, but this is out of scope for this example.
	connection.languages.diagnostics.refresh();
});

function getDocumentSettings(resource: string): Thenable<ExampleSettings> {
	if (!hasConfigurationCapability) {
		return Promise.resolve(globalSettings);
	}
	let result = documentSettings.get(resource);
	if (!result) {
		result = connection.workspace.getConfiguration({
			scopeUri: resource,
			section: "languageServerExample",
		});
		documentSettings.set(resource, result);
	}
	return result;
}

// Only keep settings for open documents
documents.onDidClose((e) => {
	documentSettings.delete(e.document.uri);
});

connection.languages.diagnostics.on(async (params) => {
	const document = documents.get(params.textDocument.uri);
	if (document !== undefined) {
		return {
			kind: DocumentDiagnosticReportKind.Full,
			items: await validateTextDocument(document),
		} satisfies DocumentDiagnosticReport;
	} else {
		// We don't know the document. We can either try to read it from disk
		// or we don't report problems for it.
		return {
			kind: DocumentDiagnosticReportKind.Full,
			items: [],
		} satisfies DocumentDiagnosticReport;
	}
});

// The content of a text document has changed. This event is emitted
// when the text document first opened or when its content has changed.
documents.onDidChangeContent((change) => {
	validateTextDocument(change.document);
});

async function validateTextDocument(
	textDocument: TextDocument,
): Promise<Diagnostic[]> {
	// Put error checking code here.
	return [];
}

connection.onDidChangeWatchedFiles((_change) => {
	// Monitored files have change in VSCode
	connection.console.log("We received a file change event");
});

// This handler provides the initial list of the completion items.
connection.onCompletion(
	(_textDocumentPosition: TextDocumentPositionParams): CompletionItem[] => {
		// The pass parameter contains the position of the text document in
		// which code complete got requested. For the example we ignore this
		// info and always provide the same completion items.
		return [];
	},
);

// This handler resolves additional information for the item selected in
// the completion list.
connection.onCompletionResolve(
	(item: CompletionItem): CompletionItem => {
		if (item.data === 1) {
			item.detail = "TypeScript details";
			item.documentation = "TypeScript documentation";
		} else if (item.data === 2) {
			item.detail = "JavaScript details";
			item.documentation = "JavaScript documentation";
		}
		return item;
	},
);

connection.languages.semanticTokens.on((params) => {
	const doc = documents.get(params.textDocument.uri);
	if (!doc) {
		return { data: [] };
	}

	const docText = doc.getText();

	const sourceTokens = FactoryAnalytics.AnalyzeSemanticTokens(docText)
		.map((x) => ({
			position: x.position,
			length: x.length,
			type: x.semanticType,
			modifier: x.modifier,
		}));

	let previousPosition: Position = { line: 0, character: 0 };
	let previousToken: typeof sourceTokens[number] | undefined = undefined;

	const tokens: number[] = [];
	for (const token of sourceTokens) {
		const position = doc.positionAt(token.position);
		if (previousToken === undefined) {
			tokens.push(
				...encodeToken(
					position.line,
					position.character,
					token.length,
					token.type,
					token.modifier,
				),
			);
		} else {
			tokens.push(
				...encodeToken(
					position.line - previousPosition.line,
					position.line === previousPosition.line
						? position.character - previousPosition.character
						: position.character,
					token.length,
					token.type,
					token.modifier,
				),
			);
		}
		previousPosition = position;
		previousToken = token;
	}

	return { data: tokens };
});

function encodeToken(
	line: number,
	char: number,
	length: number,
	tokenType: FactorySemanticType,
	tokenModifiers: number,
): number[] {
	return [line, char, length, tokenType, tokenModifiers];
}

// Make the text document manager listen on the connection
// for open, change and close text document events
documents.listen(connection);

// Listen on the connection
connection.listen();