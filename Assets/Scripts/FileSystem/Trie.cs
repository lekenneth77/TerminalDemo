using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrieNode {
    public Dictionary<char, TrieNode> children = new Dictionary<char, TrieNode>(); //i forgot i have to also deal with periods :(
    public bool isWord = false;
    public string word = ""; //i could check if word is empty but let's be normal about this
}

public class Trie
{
    public TrieNode root;
    public Trie() {
        root = new TrieNode();
    }

    public void AddWord(string word) {
        string traversalWord = word;
        if (GameController.Get && GameController.Get.TerminalType == TerminalType.Windows) {traversalWord = word.ToLower();}
        AddHelper(root, traversalWord, word, 0);
    }

    private void AddHelper(TrieNode curNode, string traversalWord, string realWord, int index) {
        if (curNode == null) {
            return;
        } else if (index >= traversalWord.Length) {
            curNode.isWord = true;
            curNode.word = realWord;
            return;
        }

        if (!curNode.children.ContainsKey(traversalWord[index])) {
            curNode.children.Add(traversalWord[index], new TrieNode());
        }

        AddHelper(curNode.children[traversalWord[index]], traversalWord, realWord, index + 1);
    }

    public bool DoesWordExist(string word) {
        if (GameController.Get && GameController.Get.TerminalType == TerminalType.Windows) {word = word.ToLower();}
        TrieNode result = FindHelper(root, word, 0);
        return result != null && result.isWord;
    }

    public List<string> GetWordsWithPrefix(string prefix) {
        if (GameController.Get && GameController.Get.TerminalType == TerminalType.Windows) {prefix = prefix.ToLower();}
        TrieNode result = FindHelper(root, prefix, 0);
        if (result == null) {
            return new List<string>();
        }
        List<string> words = new List<string>();
        GetAllWordsFromNode(result, words);
        return words;
    }

    private TrieNode FindHelper(TrieNode curNode, string word, int index) {
        if (curNode == null || index >= word.Length) {
            return curNode;
        } 

        if (curNode.children.ContainsKey(word[index])) {
            return FindHelper(curNode.children[word[index]], word, index + 1);
        }
        return null;
    }

    //given a node, gets all words that start from this node
    private void GetAllWordsFromNode(TrieNode curNode, List<string> words) {
        if (curNode == null) {
            return;
        } 

        if (curNode.isWord) {
            words.Add(curNode.word);
        }

        foreach (TrieNode n in curNode.children.Values) {
            GetAllWordsFromNode(n, words);
        }
    }

}
