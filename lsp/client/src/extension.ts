/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

import * as path from "path";
import { ExtensionContext, languages, workspace } from "vscode";

import {
	LanguageClient,
	LanguageClientOptions,
	ServerOptions,
	TransportKind,
} from "vscode-languageclient/node";

const schemes = [
	"file",
	"untitled",
	"git",
	"github",
	"azurerepos",
	"buffer",
	"zipfile",
	"vsls",
	"walkThroughSnippet",
	"vs-code-notebook-cell",
	"vscode-notebook-cell",
	"memfs",
	"vscode-vfs",
	"office-script",
];

let client: LanguageClient;

export function activate(context: ExtensionContext) {
	// The server is implemented in node
	const serverModule = context.asAbsolutePath(
		path.join("server", "out", "server.js"),
	);

	// If the extension is launched in debug mode then the debug server options are used
	// Otherwise the run options are used
	const serverOptions: ServerOptions = {
		run: { module: serverModule, transport: TransportKind.ipc },
		debug: {
			module: serverModule,
			transport: TransportKind.ipc,
		},
	};

	// Options to control the language client
	const clientOptions: LanguageClientOptions = {
		// Register the server for plain text documents
		// documentSelector: [
		// 	{ scheme: "file", language: "factory" },
		// 	{ scheme: "untitled", language: "factory" },
		// 	{
		// 		scheme: "file",
		// 		pattern: "**/*.factory",
		// 	},
		// 	{
		// 		scheme: "file",
		// 		pattern: "**/*.md",
		// 		language: "factory",
		// 	},
		// 	{
		// 		scheme: "untitled",
		// 		pattern: "**/*.md",
		// 		language: "factory",
		// 	},
		// ],
		documentSelector: schemes.flatMap(
			(
				x,
			) => [{ scheme: x, language: "factory" }, {
				scheme: x,
				language: "markdown",
			}, {
				scheme: x,
				pattern: "**/*.factory",
			}, { scheme: x, pattern: "**/*.md" }],
		),
		synchronize: {
			// Notify the server about file changes to '.clientrc files contained in the workspace
			fileEvents: workspace.createFileSystemWatcher("**/.clientrc"),
		},
	};

	// Create the language client and start the client.
	client = new LanguageClient(
		"FactoryLanguageServer",
		"Factory Language Server",
		serverOptions,
		clientOptions,
	);

	// Start the client. This will also launch the server
	client.start();
}

export function deactivate(): Thenable<void> | undefined {
	if (!client) {
		return undefined;
	}
	return client.stop();
}
