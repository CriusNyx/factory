import fs from "node:fs/promises";
import { parseCollection } from "./docs-parser/utilities";
import _, { isArray } from "lodash";

function getClassCategory(classObj: any) {
  const className = classObj.ClassName as string;
  const category = className.substring(0, className.indexOf("_"));
  return category;
}

function flattenClasses(nativeClass: any) {
  return [...nativeClass.Classes];
}

function parseUEStrings(item: any) {
  let output = {};
  for (const key of Object.keys(item)) {
    const value = item[key];
    output[key] = value;
    if (typeof value === "string") {
      if (value.startsWith("(")) {
        try {
          const parsedValue = parseCollection(value);
          output[key] = parsedValue;
        } catch {}
      }
    }
  }

  return output;
}

function humanizeField(item: any) {
  if (typeof item === "object") {
    if (_.isArray(item)) {
      return item.map(humanizeField);
    } else {
      const output = {};
      for (const key of Object.keys(item)) {
        output[humanizeFieldName(key)] =
          key === "ItemClass"
            ? humanizeSpecialField(item[key])
            : humanizeField(item[key]);
      }
      return output;
    }
  } else {
    return item;
  }
}

function humanizeSpecialField(name: string) {
  if (name.startsWith('"') && name.endsWith('"') && name.includes(".")) {
    return name
      .substring(name.lastIndexOf(".") + 1)
      .replace("'", "")
      .replace('"', "");
  } else {
    return name;
  }
}

function humanizeFieldName(name: string) {
  if (name[0] === "m" && name[1].toUpperCase() === name[1]) {
    const output = name.substring(1);
    return _.camelCase(output);
  }
  return name;
}

function fixProducedInName(classObj: any) {
  if ("producedIn" in classObj) {
    classObj.producedIn = [...classObj.producedIn].map(fixProducedInNameString);
  }
  return classObj;
}

function fixProducedInNameString(value: string) {
  value = value.replace(/\"/g, "");
  return value.substring(value.lastIndexOf(".") + 1);
}

function categorize(classes: any[]) {
  const categories = new Set(classes.map(getClassCategory));
  const output: { [key: string]: any[] } = {};
  for (const category of categories.values()) {
    output[category] = [];
  }
  for (const classObj of classes) {
    output[getClassCategory(classObj)].push(classObj);
  }
  return output;
}

async function main() {
  const json = await fs.readFile("./docs-raw.json");

  const items = JSON.parse(json.toString()) as any[];

  const classes = items
    .flatMap(flattenClasses)
    .map(parseUEStrings)
    .map(humanizeField);

  const categories = categorize(classes);

  categories.Recipe = categories.Recipe.map(fixProducedInName);

  fs.writeFile("./classes.json", JSON.stringify(classes, undefined, 2));
  fs.writeFile("./categories.json", JSON.stringify(categories, undefined, 2));

  const classNames = new Set(classes.map(getClassCategory));

  console.log(classNames);
}

main();
