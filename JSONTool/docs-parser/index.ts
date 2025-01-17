//import util from 'util';
import { DocsDataClassMap, DocsTopLevelClass } from './types';
import { categorizeDataClasses, validateClassList } from './class-categorizer';
import { parseBuildables, parseItems, parseRecipes, parseSchematics } from './parsers';

const nativeClassRegex = /FactoryGame\.(.+)'$/;

function parseDocs(input: string) {
  return parseDocsString(input);
}

function parseDocsString(input: string) {
  const docs = (JSON.parse(input) as DocsTopLevelClass[]);

  if (!Array.isArray(docs)) {
    throw new Error('Invalid Docs.json file -- not an array');
  }

  const dataClassMap: DocsDataClassMap = {};
  for (const entry of docs) {
    if (!Object.prototype.hasOwnProperty.call(entry, 'NativeClass') || !Object.prototype.hasOwnProperty.call(entry, 'Classes')) {
      throw new Error('Invalid Docs.json file -- missing required keys');
    }
    const match = nativeClassRegex.exec(entry.NativeClass);
    if (!match || !match[1]) {
      throw new Error(`Could not parse top-level class ${entry.NativeClass}`);
    }
    const nativeClassName = match[1];
    dataClassMap[nativeClassName] = entry.Classes;
  }

  const topLevelClassList = Object.keys(dataClassMap).sort();
  validateClassList(topLevelClassList);
  const categorizedDataClasses = categorizeDataClasses(dataClassMap);

  const { items, resources } = parseItems(categorizedDataClasses);
  const buildables = parseBuildables(categorizedDataClasses, { items, resources });
  const { productionRecipes, buildableRecipes, customizerRecipes } = parseRecipes(categorizedDataClasses, { items, buildables });
  const schematics = parseSchematics(categorizedDataClasses, { items, resources, productionRecipes, buildableRecipes, customizerRecipes });

  const data = {
    items,
    resources,
    buildables,
    productionRecipes,
    buildableRecipes,
    customizerRecipes,
    schematics,
  };

  validateSlugs(data);

  return {
    meta: {
      originalDocs: docs,
      topLevelClassList,
      dataClassesByTopLevelClass: dataClassMap,
      dataClassesByCategory: categorizedDataClasses,
    },
    ...data,
  };
}

const slugRegex = /^[a-z0-9-]+$/;
function validateSlugs(data: any) {
  const slugs: string[] = [];
  Object.entries<any>(data).forEach(([category, entries]) => {
    Object.entries<any>(entries).forEach(([className, classData]) => {
      if (!slugRegex.exec(classData.slug)) {
        // eslint-disable-next-line no-console
        console.warn(`WARNING: Invalid slug format: [${classData.slug}] of [${className}] from [${category}]`);
      }
      if (classData.slug) {
        if (slugs.includes(classData.slug)) {
          // eslint-disable-next-line no-console
          //console.warn(`WARNING: Duplicate global slug: [${classData.slug}] of [${className}] from [${category}]`);
        } else {
          slugs.push(classData.slug);
        }
      }
    });
  });
}

export default parseDocs;
